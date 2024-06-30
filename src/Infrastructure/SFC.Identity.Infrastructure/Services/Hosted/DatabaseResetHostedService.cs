using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SFC.Identity.Infrastructure.Extensions;
using SFC.Identity.Infrastructure.Persistence;
using SFC.Identity.Infrastructure.Services.Hosted;
using SFC.Identity.Infrastructure.Settings;

namespace SFC.Data.Infrastructure.Services.Hosted;
public class DatabaseResetHostedService(
    ILogger<DatabaseResetHostedService> logger,
    IServiceProvider services,
    IHostEnvironment hostEnvironment,
    IOptions<IdentitySettings> identitySettings) : BaseInitializationService(logger)
{
    private readonly IServiceProvider _services = services;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
    private readonly IOptions<IdentitySettings> _identitySettings = identitySettings;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Identity Initialization Hosted Service running.");

        using IServiceScope scope = _services.CreateScope();

        IdentityDbContext identityContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        ConfigurationDbContext configurationContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        if (_hostEnvironment.IsDevelopment())
        {
            PersistedGrantDbContext persistedGrantContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();            

            await identityContext.Database.EnsureDeletedAsync(cancellationToken);

            if (identityContext.Database.IsRelational())
            {
                await identityContext.Database.MigrateAsync(cancellationToken);
                await persistedGrantContext.Database.MigrateAsync(cancellationToken);
                await configurationContext.Database.MigrateAsync(cancellationToken);
            }

            await identityContext.SeedUsersAsync(cancellationToken);            
        }

        await configurationContext.EnsureIdentityConfigurationExistAsync(_identitySettings.Value, cancellationToken);

        await identityContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}