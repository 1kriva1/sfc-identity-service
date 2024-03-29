﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Infrastructure.Persistence;
using SFC.Identity.Infrastructure.Persistence.Models;
using SFC.Identity.Infrastructure.Settings;

using Xunit;

namespace SFC.Identity.Infrastructure.UnitTests;

public class InfrastructureRegistrationTests
{
    private readonly ServiceProvider _serviceProvider;

    public InfrastructureRegistrationTests()
    {
        ServiceCollection serviceCollection = new();
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new KeyValuePair<string, string?>[1] { new KeyValuePair<string, string?>("ConnectionString", "Value") })
            .Build();
        serviceCollection.AddDbContext<IdentityDbContext>();
        serviceCollection.AddLogging();
        serviceCollection.AddInfrastructureServices(configuration);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    [Trait("Registration", "Servises")]
    public void InfrastructureRegistration_Execute_IdentityServicesAreRegistered()
    {
        // Assert
        Assert.NotNull(_serviceProvider.GetService<IOptions<IdentityOptions>>());
        Assert.NotNull(_serviceProvider.GetService<UserManager<ApplicationUser>>());
        Assert.NotNull(_serviceProvider.GetService<RoleManager<ApplicationRole>>());
        Assert.NotNull(_serviceProvider.GetService<IUserStore<ApplicationUser>>());
        Assert.NotNull(_serviceProvider.GetService<IRoleStore<ApplicationRole>>());
    }

    [Fact]
    [Trait("Registration", "Servises")]
    public void InfrastructureRegistration_Execute_CustomServicesAreRegistered()
    {
        // Assert
        Assert.NotNull(_serviceProvider.GetService<IOptions<JwtSettings>>());
        Assert.NotNull(_serviceProvider.GetService<IJwtService>());
        Assert.NotNull(_serviceProvider.GetService<IIdentityService>());
        Assert.NotNull(_serviceProvider.GetService<IExistenceService>());
    }
}
