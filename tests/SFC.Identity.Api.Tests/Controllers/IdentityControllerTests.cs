using System.Security.Claims;

using AutoMapper;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using SFC.Identity.Api.Controllers;
using SFC.Identity.Api.Infrastructure.Models.Identity.Login;
using SFC.Identity.Api.Infrastructure.Models.Identity.Logout;
using SFC.Identity.Api.Infrastructure.Models.Identity.Registration;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Mappings;
using SFC.Identity.Application.Interfaces.Identity;
using SFC.Identity.Application.Interfaces.Identity.Dto.Login;
using SFC.Identity.Application.Interfaces.Identity.Dto.Logout;
using SFC.Identity.Application.Interfaces.Identity.Dto.Registration;

namespace SFC.Identity.Api.Tests.Controllers;

public class IdentityControllerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IAuthenticationService> _authenticationServiceMock = new();
    private readonly IdentityController _controller;

    public IdentityControllerTests()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        // authentication mock
        _authenticationServiceMock
            .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.FromResult((object)null!));
        _authenticationServiceMock
            .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.FromResult((object)null!));
        serviceProviderMock
            .Setup(_ => _.GetService(typeof(IAuthenticationService)))
            .Returns(_authenticationServiceMock.Object);
        // mapper mock
        IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
           .CreateMapper();
        serviceProviderMock
            .Setup(_ => _.GetService(typeof(IMapper)))
            .Returns(mapper);
        // controller mock
        _controller = new IdentityController(_identityServiceMock.Object);
        _controller.ControllerContext.HttpContext = new DefaultHttpContext { RequestServices = serviceProviderMock.Object };
    }

    [Fact]
    [Trait("API", "Controller")]
    public async Task API_Controller_Identity_Register_ShouldReturnSuccessResponse()
    {
        // Arrange
        string userName = "username", returnUrl = "https:\\localhost4200";
        RegistrationRequest request = new() { UserName = userName, Email = null, Password = "pass", ConfirmPassword = "pass", ReturnUrl = returnUrl };
        RegistrationResultDto result = new()
        {
            UserId = Guid.NewGuid(),
            UserName = userName,
            ReturnUrl = returnUrl,
            Properties = new AuthenticationProperties()
        };

        _identityServiceMock.Setup(es => es.RegisterAsync(It.IsAny<RegistrationModelDto>())).ReturnsAsync(result);

        // Act
        ActionResult<RegistrationResponse> response = await _controller.RegisterAsync(request).ConfigureAwait(false);

        // Assert
        ActionResult<RegistrationResponse> actionResult = Assert.IsType<ActionResult<RegistrationResponse>>(response);

        OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

        RegistrationResponse responseResult = Assert.IsType<RegistrationResponse>(objectResult.Value);

        Assert.True(responseResult?.Success);
        Assert.Equal(Localization.SuccessResult, responseResult?.Message);
        Assert.Equal(returnUrl, responseResult?.ReturnUrl);
        _authenticationServiceMock.Verify(um => um
            .SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()),
                Times.Once());
    }

    [Fact]
    [Trait("API", "Controller")]
    public async Task API_Controller_Identity_Login_ShouldReturnSuccessResponse()
    {
        // Arrange
        string userName = "username", returnUrl = "https:\\localhost4200";
        LoginRequest request = new() { UserName = userName, Email = null, Password = "pass", RememberMe = true, ReturnUrl = returnUrl };
        LoginResultDto result = new()
        {
            UserId = Guid.NewGuid(),
            UserName = userName,
            ReturnUrl = returnUrl,
            Properties = new AuthenticationProperties()
        };

        _identityServiceMock.Setup(es => es.LoginAsync(It.IsAny<LoginModelDto>())).ReturnsAsync(result);

        // Act
        ActionResult<LoginResponse> response = await _controller.LoginAsync(request).ConfigureAwait(false);

        // Assert
        ActionResult<LoginResponse> actionResult = Assert.IsType<ActionResult<LoginResponse>>(response);

        OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

        LoginResponse responseResult = Assert.IsType<LoginResponse>(objectResult.Value);

        Assert.True(responseResult?.Success);
        Assert.Equal(Localization.SuccessResult, responseResult?.Message);
        Assert.Equal(returnUrl, responseResult?.ReturnUrl);
        _authenticationServiceMock.Verify(um => um
            .SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()),
                Times.Once());
    }

    [Fact]
    [Trait("API", "Controller")]
    public async Task API_Controller_Identity_Logout_ShouldReturnSuccessResponseWithSignOut()
    {
        // Arrange
        string logoutId = "logout_id";
        LogoutResultDto result = new()
        {
            AutomaticRedirectAfterSignOut = true,
            ClientName = "client_name",
            PostLogoutRedirectUrl = "https:\\localhost4200",
            ShowLogoutPrompt = false,
            SignOutIFrameUrl = "https:\\localhost4200"
        };

        _identityServiceMock.Setup(es => es.LogoutAsync(It.IsAny<LogoutModelDto>())).ReturnsAsync(result);

        // Act
        ActionResult<LogoutResponse> response = await _controller.LogoutAsync(logoutId).ConfigureAwait(false);

        // Assert
        ActionResult<LogoutResponse> actionResult = Assert.IsType<ActionResult<LogoutResponse>>(response);

        OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

        LogoutResponse responseResult = Assert.IsType<LogoutResponse>(objectResult.Value);

        Assert.True(responseResult?.Success);
        Assert.Equal(Localization.SuccessResult, responseResult?.Message);
        Assert.Equal(result.SignOutIFrameUrl, responseResult?.SignOutIFrameUrl);
        Assert.Equal(result.ClientName, responseResult?.ClientName);
        Assert.Equal(result.PostLogoutRedirectUrl, responseResult?.PostLogoutRedirectUrl);
        Assert.Equal(result.AutomaticRedirectAfterSignOut, responseResult?.AutomaticRedirectAfterSignOut);
        Assert.Equal(result.ShowLogoutPrompt, responseResult?.ShowLogoutPrompt);
        _authenticationServiceMock.Verify(um => um.SignOutAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme, null), Times.Once());
        _authenticationServiceMock.Verify(um => um.SignOutAsync(It.IsAny<HttpContext>(), OpenIdConnectDefaults.AuthenticationScheme, null), Times.Once());
    }

    [Fact]
    [Trait("API", "Controller")]
    public async Task API_Controller_Identity_Logout_ShouldReturnSuccessResponseWithoutSignOut()
    {
        // Arrange
        string logoutId = "logout_id";
        LogoutResultDto result = new()
        {
            AutomaticRedirectAfterSignOut = true,
            ClientName = "client_name",
            PostLogoutRedirectUrl = "https:\\localhost4200",
            ShowLogoutPrompt = true,
            SignOutIFrameUrl = "https:\\localhost4200"
        };

        _identityServiceMock.Setup(es => es.LogoutAsync(It.IsAny<LogoutModelDto>())).ReturnsAsync(result);

        // Act
        ActionResult<LogoutResponse> response = await _controller.LogoutAsync(logoutId).ConfigureAwait(false);

        // Assert
        ActionResult<LogoutResponse> actionResult = Assert.IsType<ActionResult<LogoutResponse>>(response);

        OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

        LogoutResponse responseResult = Assert.IsType<LogoutResponse>(objectResult.Value);

        Assert.True(responseResult?.Success);
        Assert.Equal(Localization.SuccessResult, responseResult?.Message);
        Assert.Equal(result.SignOutIFrameUrl, responseResult?.SignOutIFrameUrl);
        Assert.Equal(result.ClientName, responseResult?.ClientName);
        Assert.Equal(result.PostLogoutRedirectUrl, responseResult?.PostLogoutRedirectUrl);
        Assert.Equal(result.AutomaticRedirectAfterSignOut, responseResult?.AutomaticRedirectAfterSignOut);
        Assert.Equal(result.ShowLogoutPrompt, responseResult?.ShowLogoutPrompt);
        _authenticationServiceMock.Verify(um => um.SignOutAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme, null), Times.Never());
        _authenticationServiceMock.Verify(um => um.SignOutAsync(It.IsAny<HttpContext>(), OpenIdConnectDefaults.AuthenticationScheme, null), Times.Never());
    }

    [Fact]
    [Trait("API", "Controller")]
    public async Task API_Controller_Identity_PostLogout_ShouldReturnSuccessResponse()
    {
        // Arrange
        string logoutId = "logout_id";
        LogoutResultDto result = new()
        {
            AutomaticRedirectAfterSignOut = true,
            ClientName = "client_name",
            PostLogoutRedirectUrl = "https:\\localhost4200",
            ShowLogoutPrompt = false,
            SignOutIFrameUrl = "https:\\localhost4200"
        };

        _identityServiceMock.Setup(es => es.PostLogoutAsync(It.IsAny<LogoutModelDto>())).ReturnsAsync(result);

        // Act
        ActionResult<LogoutResponse> response = await _controller.PostLogoutAsync(logoutId).ConfigureAwait(false);

        // Assert
        ActionResult<LogoutResponse> actionResult = Assert.IsType<ActionResult<LogoutResponse>>(response);

        OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

        LogoutResponse responseResult = Assert.IsType<LogoutResponse>(objectResult.Value);

        Assert.True(responseResult?.Success);
        Assert.Equal(Localization.SuccessResult, responseResult?.Message);
        Assert.Equal(result.SignOutIFrameUrl, responseResult?.SignOutIFrameUrl);
        Assert.Equal(result.ClientName, responseResult?.ClientName);
        Assert.Equal(result.PostLogoutRedirectUrl, responseResult?.PostLogoutRedirectUrl);
        Assert.Equal(result.AutomaticRedirectAfterSignOut, responseResult?.AutomaticRedirectAfterSignOut);
        Assert.Equal(result.ShowLogoutPrompt, responseResult?.ShowLogoutPrompt);
        _authenticationServiceMock.Verify(um => um.SignOutAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme, null), Times.Once());
        _authenticationServiceMock.Verify(um => um.SignOutAsync(It.IsAny<HttpContext>(), OpenIdConnectDefaults.AuthenticationScheme, null), Times.Once());
    }
}