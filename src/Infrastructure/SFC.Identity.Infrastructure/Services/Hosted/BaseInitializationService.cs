using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SFC.Data.Infrastructure.Services.Hosted;

namespace SFC.Identity.Infrastructure.Services.Hosted;
public abstract class BaseInitializationService(ILogger logger) : IHostedService
{
    protected readonly ILogger _logger = logger;

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(cancellationToken);
    }

    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
}
