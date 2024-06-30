using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using SFC.Data.Infrastructure.Services.Hosted;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Infrastructure.Configuration;
using SFC.Identity.Infrastructure.Persistence;
using SFC.Identity.Infrastructure.Settings;

using Xunit;

namespace SFC.Identity.Infrastructure.UnitTests.Services.Hosted;
public class DatabaseResetHostedServiceTests
{
    private readonly Mock<ILogger<DatabaseResetHostedService>> _loggerMock = new();
    private readonly Mock<IOptions<IdentitySettings>> _identitySettingsMock = new();

    public DatabaseResetHostedServiceTests()
    {
        _identitySettingsMock.Setup(x => x.Value).Returns(new IdentitySettings());
    }

    [Fact]
    [Trait("Service", "DatabaseResetHosted")]
    public async Task Service_Hosted_DatabaseReset_ShouldCreateDatabase()
    {
        (IHostedService, IdentityDbContext, ConfigurationDbContext, PersistedGrantDbContext) result = CreateDatabaseResetHostedService();

        // Act
        await result.Item1.StartAsync(new CancellationToken());

        // Assert
        Assert.True(result.Item2.Database.CanConnect());
        Assert.True(result.Item3.Database.CanConnect());
        Assert.True(result.Item4.Database.CanConnect());
    }

    [Fact]
    [Trait("Service", "DatabaseResetHosted")]
    public async Task Service_Hosted_DatabaseReset_ShouldEnsureIdentityConfigurationExist()
    {
        IdentitySettings assertIdentitySettings = new()
        {
            Clients = [new ClientSetting { Id = "id" }],
            Api = new ApiSettings
            {
                Resources = [new ApiResourceSetting { Name = "sfc.data" }],
                Scopes = [new ApiScopeSetting { Name = "sfc.data.full" }]
            }
        };
        _identitySettingsMock.Setup(x => x.Value).Returns(assertIdentitySettings);
        (IHostedService, IdentityDbContext, ConfigurationDbContext, PersistedGrantDbContext) result = CreateDatabaseResetHostedService();

        // Act
        await result.Item1.StartAsync(new CancellationToken());

        // Assert
        Assert.Equal(assertIdentitySettings.Api.Resources.Count, result.Item3.ApiResources.Count());
        Assert.Equal(assertIdentitySettings.Api.Scopes.Count, result.Item3.ApiScopes.Count());
        Assert.Equal(assertIdentitySettings.Clients.Count, result.Item3.Clients.Count());
        Assert.Equal(IdentityConfiguration.IdentityResources.Count(), result.Item3.IdentityResources.Count());
    }

    private (IHostedService, IdentityDbContext, ConfigurationDbContext, PersistedGrantDbContext) CreateDatabaseResetHostedService()
    {
        IServiceCollection services = new ServiceCollection();

        IdentityDbContext identityContext = new(new DbContextOptionsBuilder<IdentityDbContext>()
           .UseInMemoryDatabase($"DatabaseResetHostedServiceTestsDb_{DateTime.Now.ToFileTimeUtc()}")
           .Options);
        services.AddSingleton(identityContext);

        DbContextOptionsBuilder<ConfigurationDbContext> configurationContextBuilder = new();
        configurationContextBuilder.UseInMemoryDatabase($"DatabaseResetHostedServiceTestsDb_{DateTime.Now.ToFileTimeUtc()}");
        ConfigurationDbContext configurationContext = new(configurationContextBuilder.Options)
        {
            StoreOptions = new ConfigurationStoreOptions
            {
                DefaultSchema = DbConstants.DEFAULT_SCHEMA_NAME
            }
        };
        services.AddSingleton(configurationContext);

        DbContextOptionsBuilder<PersistedGrantDbContext> persistedGrantContextBuilder = new();
        persistedGrantContextBuilder.UseInMemoryDatabase($"DatabaseResetHostedServiceTestsDb_{DateTime.Now.ToFileTimeUtc()}");
        PersistedGrantDbContext persistedGrantContext = new(persistedGrantContextBuilder.Options)
        {
            StoreOptions = new OperationalStoreOptions
            {
                DefaultSchema = DbConstants.DEFAULT_SCHEMA_NAME
            }
        };
        services.AddSingleton(persistedGrantContext);

        Mock<IHostEnvironment> hostEnvironmentMock = new();
        hostEnvironmentMock.Setup(m => m.EnvironmentName).Returns("Development");

        return (new DatabaseResetHostedService(_loggerMock.Object, services.BuildServiceProvider(), hostEnvironmentMock.Object, _identitySettingsMock.Object),
            identityContext, configurationContext, persistedGrantContext);
    }
}
