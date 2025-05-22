using System.Diagnostics;

using MediatR;

using Microsoft.Extensions.Logging;

using SFC.Identity.Application.Features.Common.Base;

namespace SFC.Identity.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, BaseRequest
{
    private readonly Stopwatch _timer = new();
    private readonly ILogger<TRequest> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        TResponse? response;

        _timer.Start();

        try
        {
            ArgumentNullException.ThrowIfNull(next);

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            response = await next();//.ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        }
        finally
        {
            _timer.Stop();

            Action<ILogger, Exception?> logTime = LoggerMessage.Define(LogLevel.Debug, request.EventId,
                $"Execution time for {typeof(TRequest).Name} is {_timer.ElapsedMilliseconds}ms.");
            logTime(_logger, null);
        }

        return response;
    }
}