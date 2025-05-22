//using Duende.IdentityServer.Services;
//using Duende.IdentityServer.Validation;

//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;

//using SFC.Identity.Application.Interfaces.Existence;
//using SFC.Identity.Application.Interfaces.Identity;
//using SFC.Identity.Infrastructure.Persistence.Context;
//using SFC.Identity.Infrastructure.Persistence.Entities;
//using SFC.Identity.Infrastructure.Services.Hosted;
//using SFC.Identity.Infrastructure.Settings;

//namespace SFC.Identity.Infrastructure.UnitTests;

//public class InfrastructureRegistrationTests
//{
//    private readonly WebApplicationBuilder _builder = WebApplication.CreateBuilder();
//    private readonly ServiceCollection _serviceCollection = new();
//    private readonly ServiceProvider _serviceProvider;

//    public InfrastructureRegistrationTests()
//    {
//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(
//                [new("ConnectionString", "Value")])
//            .Build();
//        _serviceCollection.AddDbContext<IdentityDbContext>();
//        _serviceCollection.AddLogging();
//        _builder.AddInfrastructureServices();
//        _serviceProvider = _serviceCollection.BuildServiceProvider();
//    }

//    [Fact]
//    [Trait("Registration", "Servises")]
//    public void InfrastructureRegistration_Execute_IdentityServicesAreRegistered()
//    {
//        // Assert
//        Assert.NotNull(_serviceProvider.GetService<IOptions<IdentityOptions>>());
//        Assert.NotNull(_serviceProvider.GetService<UserManager<ApplicationUser>>());
//        Assert.NotNull(_serviceProvider.GetService<RoleManager<ApplicationRole>>());
//        Assert.NotNull(_serviceProvider.GetService<IUserStore<ApplicationUser>>());
//        Assert.NotNull(_serviceProvider.GetService<IRoleStore<ApplicationRole>>());
//        Assert.NotNull(_serviceProvider.GetService<IExtensionGrantValidator>());
//        Assert.NotNull(_serviceProvider.GetService<IProfileService>());
//    }

//    [Fact]
//    [Trait("Registration", "Servises")]
//    public void InfrastructureRegistration_Execute_CustomServicesAreRegistered()
//    {
//        // Assert
//        Assert.NotNull(_serviceProvider.GetService<IOptions<IdentitySettings>>());
//        Assert.NotNull(_serviceProvider.GetService<IIdentityService>());
//        Assert.NotNull(_serviceProvider.GetService<IExistenceService>());
//        Assert.NotNull(_serviceCollection.FirstOrDefault(s => s.ImplementationType == typeof(DatabaseResetHostedService)));
//    }
//}