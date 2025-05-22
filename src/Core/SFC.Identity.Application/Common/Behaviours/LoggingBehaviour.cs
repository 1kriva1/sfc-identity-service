using System.Text.Json;

using MediatR;

using Microsoft.Extensions.Logging;

using SFC.Identity.Application.Features.Common.Base;
using SFC.Identity.Application.Interfaces.Identity;

namespace SFC.Identity.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, IUserService userService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, BaseRequest
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger = logger;

    private readonly IUserService _userService = userService;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Guid? userId = _userService.GetUserId();

        //Request
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(LogLevel.Information, request.EventId,
            $"Handling {typeof(TRequest).Name} for user {userId}.");
        logRequest(_logger, null);

        string jsonRequest = JsonSerializer.Serialize(request);

        Action<ILogger, Exception?> logSerializedRequest = LoggerMessage.Define(LogLevel.Debug, request.EventId,
            $"Request: {null}");
        logSerializedRequest(_logger, null);

        ArgumentNullException.ThrowIfNull(next);

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        TResponse response = await next();//.ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        //Response
        Action<ILogger, Exception?> logResponse = LoggerMessage.Define(LogLevel.Information, request.EventId,
           $"Handled {typeof(TResponse).Name} for user {userId}.");
        logResponse(_logger, null);

        return response;
    }
}