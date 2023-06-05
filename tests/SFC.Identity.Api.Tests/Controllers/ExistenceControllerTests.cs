using Microsoft.AspNetCore.Mvc;
using Moq;
using SFC.Identity.Api.Controllers;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Existence;
using Xunit;

namespace SFC.Identity.Api.Tests.Controllers
{
    public class ExistenceControllerTests
    {
        private readonly Mock<IExistenceService> _existenceServiceMock = new();

        private readonly ExistenceController _controller;

        public ExistenceControllerTests()
        {
            _controller = new ExistenceController(_existenceServiceMock.Object);
        }

        [Fact]
        [Trait("Existence", "CheckByUserName")]
        public async Task Existence_CheckByUserName_ShouldReturnSuccessResponse()
        {
            // Arrange
            string userName = "username";

            _existenceServiceMock.Setup(es => es.CheckByUserNameAsync(userName)).ReturnsAsync(new ExistenceResponse { Exist = true });

            // Act
            ActionResult<ExistenceResponse> result = await _controller.CheckExistenceByUserName(userName);

            // Assert
            AssertResponse(result);
        }

        [Fact]
        [Trait("Existence", "CheckByEmail")]
        public async Task Existence_CheckByEmail_ShouldReturnSuccessResponse()
        {
            // Arrange
            string email = "email@mail.com";

            _existenceServiceMock.Setup(es => es.CheckByEmailAsync(email)).ReturnsAsync(new ExistenceResponse { Exist = true });

            // Act
            ActionResult<ExistenceResponse> result = await _controller.CheckExistenceByEmail(email);

            // Assert
            AssertResponse(result);
        }

        private static void AssertResponse(ActionResult<ExistenceResponse> result)
        {
            ActionResult<ExistenceResponse> actionResult = Assert.IsType<ActionResult<ExistenceResponse>>(result);

            OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            ExistenceResponse response = Assert.IsType<ExistenceResponse>(objectResult.Value);

            Assert.True(response?.Success);
            Assert.Equal("Success result.", response?.Message);
            Assert.True(response?.Exist);
        }
    }
}
