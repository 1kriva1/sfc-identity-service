using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SFC.Identity.Application.Common.Enums;
using SFC.Identity.Application.Interfaces.Identity;

namespace SFC.Identity.Infrastructure.Services.Hosted;
public class DataInitializationHostedService(
    ILogger<DataInitializationHostedService> logger,
    IHostEnvironment hostEnvironment,
    IServiceProvider services) : BaseInitializationService(logger)
{
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
    private readonly IServiceProvider _services = services;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        EventId eventId = new((int)RequestId.DatabaseInitialize, Enum.GetName(RequestId.DatabaseInitialize));
        Action<ILogger, Exception?> logStartExecution = LoggerMessage.Define(LogLevel.Information, eventId,
            "Data Initialization Hosted Service running.");
        logStartExecution(Logger, null);

        using IServiceScope scope = _services.CreateScope();

        if (_hostEnvironment.IsDevelopment())
        {
            IUserSeedService usersSeedService = scope.ServiceProvider.GetRequiredService<IUserSeedService>();

            await usersSeedService.SeedUsersAsync(cancellationToken).ConfigureAwait(true);
        }
    }
}