﻿using Duende.IdentityServer.EntityFramework.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SFC.Identity.Application.Common.Enums;
using SFC.Identity.Infrastructure.Extensions;
using SFC.Identity.Infrastructure.Persistence.Contexts;
using SFC.Identity.Infrastructure.Settings;

namespace SFC.Identity.Infrastructure.Services.Hosted;
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
        EventId eventId = new((int)RequestId.DatabaseReset, Enum.GetName(RequestId.DatabaseReset));
        Action<ILogger, Exception?> logStartExecution = LoggerMessage.Define(LogLevel.Information, eventId,
            "Data Initialization Hosted Service running.");
        logStartExecution(Logger, null);

        using IServiceScope scope = _services.CreateScope();

        IdentityDbContext identityContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        ConfigurationDbContext configurationContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        PersistedGrantDbContext persistedGrantContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

        if (_hostEnvironment.IsDevelopment())
        {
            await identityContext.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(true);
            await persistedGrantContext.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(true);
            await configurationContext.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(true);
        }

        if (_hostEnvironment.IsTesting())
        {
            await EnsureCreatedAsync(identityContext, cancellationToken).ConfigureAwait(true);
            await EnsureCreatedAsync(persistedGrantContext, cancellationToken).ConfigureAwait(true);
            await EnsureCreatedAsync(configurationContext, cancellationToken).ConfigureAwait(true);
        }
        else
        {
            await identityContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(true);
            await persistedGrantContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(true);
            await configurationContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(true);
        }

        await configurationContext.EnsureIdentityConfigurationExistAsync(_identitySettings.Value, cancellationToken).ConfigureAwait(true);
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