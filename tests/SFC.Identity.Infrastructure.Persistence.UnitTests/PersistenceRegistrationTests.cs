using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFC.Identity.Infrastructure.Persistence.UnitTests;
public class PersistenceRegistrationTests
{
    [Fact]
    [Trait("Registration", "Servises")]
    public void PersistenceRegistration_Execute_ServicesAreRegistered()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder()
           .AddInMemoryCollection(
               new KeyValuePair<string, string?>[1] { new KeyValuePair<string, string?>("ConnectionString", "Value") })
           .Build();
        ServiceCollection serviceCollection = new();
        serviceCollection.AddPersistenceServices(configuration);

        // Act
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IdentityDbContext>());
    }
}
