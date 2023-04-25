using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SFC.Identity.Application.UnitTests
{
    public class ApplicationRegistrationTests
    {
        [Fact]
        public void ApplicationRegistration_Execute_ServicesAreRegistered()
        {
            // Arrange
            ServiceCollection serviceCollection = new();

            // Act
            serviceCollection.AddApplicationServices();
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            // Assert
            Assert.NotNull(serviceProvider.GetService<IMediator>());
            Assert.NotNull(serviceProvider.GetService<IMapper>());
        }
    }
}
