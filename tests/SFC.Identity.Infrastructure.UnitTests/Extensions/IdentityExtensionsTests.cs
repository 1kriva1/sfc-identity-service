using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFC.Identity.Infrastructure.Extensions;

using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SFC.Identity.Infrastructure.Persistence.Models;
using SFC.Identity.Infrastructure.Persistence;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using SFC.Identity.Infrastructure.Settings;
using Duende.IdentityServer.EntityFramework.Options;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Infrastructure.Configuration;

namespace SFC.Identity.Infrastructure.UnitTests.Extensions;
public class IdentityExtensionsTests
{
    [Fact]
    [Trait("Extension", "Identity")]
    public void Extension_Identity_ShouldAddIdentity()
    {
        //Arrange
        ServiceCollection serviceCollection = new();
        serviceCollection.AddDbContext<IdentityDbContext>();
        IConfiguration configuration = new ConfigurationBuilder().Build();

        // Act
        serviceCollection.AddIdentity(configuration);
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IOptions<IdentityOptions>>());
        Assert.NotNull(serviceProvider.GetService<UserManager<ApplicationUser>>());
        Assert.NotNull(serviceProvider.GetService<RoleManager<ApplicationRole>>());
        Assert.NotNull(serviceProvider.GetService<IUserStore<ApplicationUser>>());
        Assert.NotNull(serviceProvider.GetService<IRoleStore<ApplicationRole>>());
        Assert.NotNull(serviceProvider.GetService<IExtensionGrantValidator>());
        Assert.NotNull(serviceProvider.GetService<IProfileService>());
    }

    [Fact]
    [Trait("Extension", "Identity")]
    public async Task Extension_Identity_ShouldEnsureIdentityResourcesExist()
    {
        //Arrange
        ConfigurationDbContext context = CreateConfigurationDbContext();

        // Act
        await context.EnsureIdentityConfigurationExistAsync(new IdentitySettings(), CancellationToken.None);

        // Assert
        Assert.Equal(IdentityConfiguration.IdentityResources.Count(), context.IdentityResources.Count());
    }

    [Fact]
    [Trait("Extension", "Identity")]
    public async Task Extension_Identity_ShouldEnsureApiResourcesExist()
    {
        //Arrange
        ConfigurationDbContext context = CreateConfigurationDbContext();
        ICollection<ApiResourceSetting> assertApiResources = [new() { Name = "sfc.data" }];

        // Act
        await context.EnsureIdentityConfigurationExistAsync(new IdentitySettings { 
            Api = new ApiSettings { Resources = assertApiResources } 
        }, CancellationToken.None);

        // Assert
        Assert.Equal(assertApiResources.Count, context.ApiResources.Count());
    }

    [Fact]
    [Trait("Extension", "Identity")]
    public async Task Extension_Identity_ShouldEnsureApiScopesExist()
    {
        //Arrange
        ConfigurationDbContext context = CreateConfigurationDbContext();
        ICollection<ApiScopeSetting> assertApiScopes = [new() { Name = "sfc.data.full" }];

        // Act
        await context.EnsureIdentityConfigurationExistAsync(new IdentitySettings
        {
            Api = new ApiSettings { Scopes = assertApiScopes }
        }, CancellationToken.None);

        // Assert
        Assert.Equal(assertApiScopes.Count, context.ApiScopes.Count());
    }

    [Fact]
    [Trait("Extension", "Identity")]
    public async Task Extension_Identity_ShouldEnsureClientsExist()
    {
        //Arrange
        ConfigurationDbContext context = CreateConfigurationDbContext();
        ICollection<ClientSetting> assertClients = [new() {  Name = "Name", Id = "sfc" }];

        // Act
        await context.EnsureIdentityConfigurationExistAsync(new IdentitySettings
        {
            Clients = assertClients
        }, CancellationToken.None);

        // Assert
        Assert.Equal(assertClients.Count, context.Clients.Count());
    }

    private static ConfigurationDbContext CreateConfigurationDbContext()
    {
        DbContextOptionsBuilder<ConfigurationDbContext> builder = new();
        builder.UseInMemoryDatabase($"IdentityExtensionsTestsDb_{DateTime.Now.ToFileTimeUtc()}");

        return new(builder.Options)
        {
            StoreOptions = new ConfigurationStoreOptions
            {
                DefaultSchema = DbConstants.DEFAULT_SCHEMA_NAME
            }
        };
    }
}
