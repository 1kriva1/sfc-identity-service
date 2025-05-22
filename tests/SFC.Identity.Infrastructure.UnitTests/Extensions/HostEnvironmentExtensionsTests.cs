using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

using SFC.Identity.Infrastructure.Extensions;

using Environments = SFC.Identity.Infrastructure.Constants.Environments;

namespace SFC.Identity.Infrastructure.UnitTests.Extensions;
public class HostEnvironmentExtensionsTests
{
    [Fact]
    [Trait("Extension", "HostEnvironment")]
    public void Extension_HostEnvironment_ShouldReturnIsTesting()
    {
        //Arrange
        IHostEnvironment environment = new HostingEnvironment
        {
            EnvironmentName = Environments.Testing
        };

        // Act
        bool result = environment.IsTesting();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("Extension", "HostEnvironment")]
    public void Extension_HostEnvironment_ShouldReturnIsNotTesting()
    {
        //Arrange
        IHostEnvironment environment = new HostingEnvironment
        {
            EnvironmentName = "Not_Testing"
        };

        // Act
        bool result = environment.IsTesting();

        // Assert
        Assert.False(result);
    }
}