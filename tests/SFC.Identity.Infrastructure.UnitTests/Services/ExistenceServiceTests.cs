using Microsoft.AspNetCore.Identity;
using Moq;
using SFC.Identity.Application.Models.Existence;
using SFC.Identity.Infrastructure.Persistence.Models;
using SFC.Identity.Infrastructure.Services;
using Xunit;

namespace SFC.Identity.Infrastructure.UnitTests.Services
{
    public class ExistenceServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        private readonly ExistenceService _service;

        public ExistenceServiceTests()
        {
            Mock<IUserStore<ApplicationUser>> userStore = new();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            _service = new ExistenceService(_userManagerMock.Object);
        }

        [Fact]
        public async Task ExistenceService_ByUserName_ShouldReturnPlayerExist()
        {
            // Arrange
            string username = "username";

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

            // Act
            ExistenceResponse response = await _service.CheckByUserNameAsync(username);

            // Assert
            Assert.True(response.Exist);
        }

        [Fact]
        public async Task ExistenceService_ByUserName_ShouldReturnPlayerNotExist()
        {
            // Arrange
            string username = "username";

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

            // Act
            ExistenceResponse response = await _service.CheckByUserNameAsync("username_another");

            // Assert
            Assert.False(response.Exist);
        }

        [Fact]
        public async Task ExistenceService_ByEmail_ShouldReturnPlayerExist()
        {
            // Arrange
            string email = "email@mail.com";

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

            // Act
            ExistenceResponse response = await _service.CheckByEmailAsync(email);
             
            // Assert
            Assert.True(response.Exist);
        }

        [Fact]
        public async Task ExistenceService_ByEmail_ShouldReturnPlayerNotExist()
        {
            // Arrange
            string email = "email@mail.com";

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

            // Act
            ExistenceResponse response = await _service.CheckByEmailAsync("email_another@mail.com");

            // Assert
            Assert.False(response.Exist);
        }
    }
}
