using Microsoft.AspNetCore.Mvc;
using Moq;
using SFC.Identity.Api.Controllers;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.RefreshToken;
using SFC.Identity.Application.Models.Registration;
using SFC.Identity.Application.Models.Tokens;
using Xunit;

namespace SFC.Identity.Api.Tests.Controllers
{
    public class IdentityControllerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;

        private readonly IdentityController _controller;

        public IdentityControllerTests()
        {
            _identityServiceMock = new();
            _controller = new IdentityController(_identityServiceMock.Object);
        }

        [Fact]
        [Trait("Identity", "Register")]
        public async Task Identity_Register_ShouldReturnSuccessResponse()
        {
            // Arrange
            RegistrationRequest request = new() { UserName = null, Email = null, Password = "pass", ConfirmPassword = "pass" };

            RegistrationResponse response = new()
            {
                UserId = Guid.NewGuid(),
                Token = new JwtToken()
            };

            _identityServiceMock.Setup(es => es.RegisterAsync(request)).ReturnsAsync(response);

            // Act
            ActionResult<RegistrationResponse> result = await _controller.RegisterAsync(request);

            // Assert
            ActionResult<RegistrationResponse> actionResult = Assert.IsType<ActionResult<RegistrationResponse>>(result);

            OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            RegistrationResponse responseResult = Assert.IsType<RegistrationResponse>(objectResult.Value);

            Assert.True(responseResult?.Success);
            Assert.Equal(response, responseResult);
        }

        [Fact]
        [Trait("Identity", "Login")]
        public async Task Identity_Login_ShouldReturnSuccessResponse()
        {
            // Arrange
            LoginRequest request = new() { UserName = null, Email = null, Password = "pass" };

            LoginResponse response = new()
            {
                UserId = Guid.NewGuid(),
                Token = new JwtToken()
            };

            _identityServiceMock.Setup(es => es.LoginAsync(request)).ReturnsAsync(response);

            // Act
            ActionResult<LoginResponse> result = await _controller.LoginAsync(request);

            // Assert
            ActionResult<LoginResponse> actionResult = Assert.IsType<ActionResult<LoginResponse>>(result);

            OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            LoginResponse responseResult = Assert.IsType<LoginResponse>(objectResult.Value);

            Assert.True(responseResult?.Success);
            Assert.Equal(response, responseResult);
        }

        [Fact]
        [Trait("Identity", "Logout")]
        public async Task Identity_Logout_ShouldReturnSuccessResponse()
        {
            // Arrange
            LogoutRequest request = new() { UserId = Guid.NewGuid().ToString() };

            LogoutResponse response = new ();

            _identityServiceMock.Setup(es => es.LogoutAsync(request)).ReturnsAsync(response);

            // Act
            ActionResult<LogoutResponse> result = await _controller.LogoutAsync(request);

            // Assert
            ActionResult<LogoutResponse> actionResult = Assert.IsType<ActionResult<LogoutResponse>>(result);

            OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            LogoutResponse responseResult = Assert.IsType<LogoutResponse>(objectResult.Value);

            Assert.True(responseResult?.Success);
            Assert.Equal(response, responseResult);
        }

        [Fact]
        [Trait("Identity", "RefreshToken")]
        public async Task Identity_RefreshToken_ShouldReturnSuccessResponse()
        {
            // Arrange
            RefreshTokenRequest request = new() { Token = new JwtToken() };

            RefreshTokenResponse response = new();

            _identityServiceMock.Setup(es => es.RefreshTokenAsync(request)).ReturnsAsync(response);

            // Act
            ActionResult<RefreshTokenResponse> result = await _controller.RefreshTokenAsync(request);

            // Assert
            ActionResult<RefreshTokenResponse> actionResult = Assert.IsType<ActionResult<RefreshTokenResponse>>(result);

            OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            RefreshTokenResponse responseResult = Assert.IsType<RefreshTokenResponse>(objectResult.Value);

            Assert.True(responseResult?.Success);
            Assert.Equal(response, responseResult);
        }
    }
}
