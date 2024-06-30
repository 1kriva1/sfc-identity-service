using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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

        PersistedGrantDbContext persistedGrantContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

        if (_hostEnvironment.IsDevelopment())
        {
            await identityContext.Database.EnsureDeletedAsync(cancellationToken);
            await persistedGrantContext.Database.EnsureDeletedAsync(cancellationToken);
            await configurationContext.Database.EnsureDeletedAsync(cancellationToken);
        }

        if (_hostEnvironment.IsTesting())
        {
            await EnsureCreatedAsync(identityContext, cancellationToken);
            await EnsureCreatedAsync(persistedGrantContext, cancellationToken);
            await EnsureCreatedAsync(configurationContext, cancellationToken);
        }
        else
        {
            await identityContext.Database.MigrateAsync(cancellationToken);
            await persistedGrantContext.Database.MigrateAsync(cancellationToken);
            await configurationContext.Database.MigrateAsync(cancellationToken);
        }

        if (_hostEnvironment.IsDevelopment())
        {
            await identityContext.SeedUsersAsync(cancellationToken);
        }

        await configurationContext.EnsureIdentityConfigurationExistAsync(_identitySettings.Value, cancellationToken);
    }

    private static Task EnsureCreatedAsync<C>(C context, CancellationToken cancellationToken) where C : DbContext
    {
        if (!context.Database.IsRelational())
        {
            return Task.CompletedTask;
        }

        IRelationalDatabaseCreator creator = context.GetService<IRelationalDatabaseCreator>();
        return creator.CreateTablesAsync(cancellationToken);
    }
}