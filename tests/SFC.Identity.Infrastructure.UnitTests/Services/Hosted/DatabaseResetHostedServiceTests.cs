using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Moq;

using SFC.Data.Infrastructure.Services.Hosted;
using SFC.Identity.Infrastructure.Persistence;

using Xunit;

namespace SFC.Identity.Infrastructure.UnitTests.Services.Hosted;
public class DatabaseResetHostedServiceTests
{
    private readonly Mock<ILogger<DatabaseResetHostedService>> _loggerMock = new();
    private readonly DbContextOptions<IdentityDbContext> _dbContextOptions;

    public DatabaseResetHostedServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<IdentityDbContext>()
           .UseInMemoryDatabase($"DatabaseResetHostedServiceTestsDb_{DateTime.Now.ToFileTimeUtc()}")
           .Options;
    }

    [Fact]
    [Trait("Service", "DatabaseResetHosted")]
    public async Task Service_Hosted_DatabaseReset_ShouldCreateDatabase()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        IdentityDbContext context = new(_dbContextOptions);
        services.AddSingleton(context);
        Mock<IHostEnvironment> hostEnvironmentMock = new();
        hostEnvironmentMock.Setup(m => m.EnvironmentName).Returns("Development");

        IHostedService service = new DatabaseResetHostedService(_loggerMock.Object, services.BuildServiceProvider(), hostEnvironmentMock.Object);

        // Act
        await service.StartAsync(new CancellationToken());

        // Assert
        Assert.True(context.Database.CanConnect());
    }
}
