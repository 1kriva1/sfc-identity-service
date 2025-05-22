using MediatR;

using Microsoft.Extensions.Logging;

using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Features.Common.Base;

namespace SFC.Identity.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, BaseRequest
{
    private readonly ILogger<TRequest> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            ArgumentNullException.ThrowIfNull(next);

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            return await next();//.ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        }
        catch (Exception ex) when (ex is not (AuthorizationException or BadRequestException or NotFoundException))
        {
            Action<ILogger, Exception> logError = LoggerMessage.Define(LogLevel.Error, request.EventId,
                $"Unhandled Exception for {typeof(TRequest).Name}");
            logError(_logger, ex);

            throw;
        }
    }
}