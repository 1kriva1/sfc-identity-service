using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Moq;

using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.Logout;
using SFC.Identity.Application.Models.Registration;
using SFC.Identity.Infrastructure.Persistence.Models;
using SFC.Identity.Infrastructure.Services;
using SFC.Identity.Infrastructure.Settings;

using Client = Duende.IdentityServer.Models.Client;
using LogoutRequest = Duende.IdentityServer.Models.LogoutRequest;

namespace SFC.Identity.Infrastructure.UnitTests.Services;

public class IdentityServiceTests
{
    private Mock<UserManager<ApplicationUser>> _userManagerMock = default!;

    private Mock<SignInManager<ApplicationUser>> _signInManagerMock = default!;

    private Mock<IIdentityServerInteractionService> _identityServerInteractionServiceMock = default!;

    private Mock<IServerUrls> _serverUrlsMock = default!;

    private Mock<IEventService> _eventServiceMock = default!;

    private IdentityService _service = default!;

    public IdentityServiceTests()
    {
        IdentitySettings settings = BuildIdentitySettings();
        InitMocks(settings);
    }

    #region Registration

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Register_ShouldReturnConflictExceptionIfUserAlreadyExistByUserName()
    {
        // Arrange
        string username = "username";

        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

        RegistrationModel model = new()
        {
            UserName = username,
            Password = "password",
            ConfirmPassword = "password"
        };

        // Assert
        ConflictException exception = await Assert.ThrowsAsync<ConflictException>(async () => await _service.RegisterAsync(model));
        Assert.Equal(Messages.UserAlreadyExist, exception.Message);
    }

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Register_ShouldReturnConflictExceptionIfUserAlreadyExistByEmail()
    {
        // Arrange
        string email = "email@mail.com";

        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

        RegistrationModel model = new()
        {
            Email = email,
            Password = "password",
            ConfirmPassword = "password"
        };

        // Assert
        ConflictException exception = await Assert.ThrowsAsync<ConflictException>(async () => await _service.RegisterAsync(model));
        Assert.Equal(Messages.UserAlreadyExist, exception.Message);
    }

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Register_ShouldReturnIdentityExceptionIfProcessNotSuccess()
    {
        // Arrange
        string email = "email@mail.com", username = "username", errorCode = "100", errorDescription = "100 error description",
            password = "password";

        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync((ApplicationUser)null!);

        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync((ApplicationUser)null!);

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .Returns(Task.FromResult(IdentityResult.Failed([
                new() { Code =errorCode, Description = errorDescription }
            ])));

        RegistrationModel model = new()
        {
            Email = email,
            UserName = username,
            Password = password,
            ConfirmPassword = password
        };

        // Assert
        IdentityException exception = await Assert.ThrowsAsync<IdentityException>(async () => await _service.RegisterAsync(model));
        Assert.Equal(Messages.UserRegistrationError, exception.Message);
        Assert.True(exception.Errors.ContainsKey(errorCode));
        Assert.Single(exception.Errors[errorCode]);
        Assert.Equal(errorDescription, exception.Errors[errorCode].FirstOrDefault());
    }

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Register_ShouldReturnAuthorizationExceptionIfLoginAfterRegistrationNotSuccess()
    {
        // Arrange
        string email = "email@mail.com", username = "username", password = "password",
            returnUrl = "https:\\localhost:4200", clientId = "sfc";

        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync((ApplicationUser)null!);

        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync((ApplicationUser)null!);

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .Returns(Task.FromResult(IdentityResult.Success));

        _signInManagerMock.Setup(um => um.PasswordSignInAsync(username, password, false, true)).ReturnsAsync(SignInResult.Failed);

        _identityServerInteractionServiceMock.Setup(mock => mock.GetAuthorizationContextAsync(returnUrl)).ReturnsAsync(new AuthorizationRequest
        {
            Client = new Client { ClientId = clientId }
        });

        RegistrationModel model = new()
        {
            Email = email,
            UserName = username,
            Password = password,
            ConfirmPassword = password
        };

        // Assert
        AuthorizationException exception = await Assert.ThrowsAsync<AuthorizationException>(async () => await _service.RegisterAsync(model));
        Assert.Equal(Messages.AuthorizationError, exception.Message);
        _eventServiceMock.Verify(um => um.RaiseAsync(It.IsAny<UserLoginFailureEvent>()), Times.Once());
    }

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Register_ShouldReturnSuccessResult()
    {
        // Arrange
        string email = "email@mail.com", username = "username", password = "password",
            returnUrl = "https:\\localhost:4200", clientId = "sfc";

        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync((ApplicationUser)null!);

        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync((ApplicationUser)null!);

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .Returns(Task.FromResult(IdentityResult.Success));

        _signInManagerMock.Setup(um => um.PasswordSignInAsync(username, password, false, true)).ReturnsAsync(SignInResult.Success);

        _identityServerInteractionServiceMock.Setup(mock => mock.GetAuthorizationContextAsync(returnUrl)).ReturnsAsync(new AuthorizationRequest
        {
            Client = new Client { ClientId = clientId }
        });

        RegistrationModel model = new()
        {
            Email = email,
            UserName = username,
            Password = password,
            ConfirmPassword = password,
            ReturnUrl = returnUrl
        };

        // Act
        RegistrationResult result = await _service.RegisterAsync(model);

        // Assert
        Assert.Equal(returnUrl, result.ReturnUrl);
        Assert.Equal(username, result.UserName);
        Assert.Equal(Guid.Empty, result.UserId);

        AuthenticationProperties properties = (AuthenticationProperties)result.Properties;
        Assert.False(properties.IsPersistent);
        Assert.NotNull(properties.ExpiresUtc);
        _eventServiceMock.Verify(um => um.RaiseAsync(It.IsAny<UserLoginSuccessEvent>()), Times.Once());
    }

    #endregion Registration

    #region Login

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Login_ShouldReturnAuthorizationExceptionIfUserNotFound()
    {
        // Arrange
        string username = "username", email = "email@mail.com";

        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

        LoginModel model = new()
        {
            UserName = "username_test",
            Password = "password",
            Email = "email_test@mail.com"
        };

        // Assert
        AuthorizationException exception = await Assert.ThrowsAsync<AuthorizationException>(async () => await _service.LoginAsync(model));
        Assert.Equal(Messages.AuthorizationError, exception.Message);
        _eventServiceMock.Verify(um => um.RaiseAsync(It.IsAny<UserLoginFailureEvent>()), Times.Once());
    }

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Login_ShouldReturnAuthorizationExceptionIfLoginFailed()
    {
        // Arrange
        string username = "username", email = "email@mail.com", password = "password";

        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

        _signInManagerMock.Setup(um => um.PasswordSignInAsync(username, password, false, true)).ReturnsAsync(SignInResult.Failed);

        LoginModel model = new()
        {
            UserName = username,
            Password = password,
            Email = email
        };

        // Assert
        AuthorizationException exception = await Assert.ThrowsAsync<AuthorizationException>(async () => await _service.LoginAsync(model));
        Assert.Equal(Messages.AuthorizationError, exception.Message);
        _eventServiceMock.Verify(um => um.RaiseAsync(It.IsAny<UserLoginFailureEvent>()), Times.Once());
    }

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Login_ShouldReturnForbiddenExceptionIfUserLockedOut()
    {
        // Arrange
        string username = "username", email = "email@mail.com", password = "password";

        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

        _signInManagerMock.Setup(um => um.PasswordSignInAsync(username, password, false, true)).ReturnsAsync(SignInResult.LockedOut);

        LoginModel model = new()
        {
            UserName = username,
            Password = password,
            Email = email
        };

        // Assert
        ForbiddenException exception = await Assert.ThrowsAsync<ForbiddenException>(async () => await _service.LoginAsync(model));
        Assert.Equal(Messages.AccountLocked, exception.Message);
        _eventServiceMock.Verify(um => um.RaiseAsync(It.IsAny<UserLoginFailureEvent>()), Times.Once());
    }

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Login_ShouldReturnSuccessResult()
    {
        // Arrange
        string email = "email@mail.com", username = "username", password = "password",
            returnUrl = "https:\\localhost:4200", clientId = "sfc";
        bool rememberMe = true;

        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser { UserName = username });

        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser { UserName = username });

        _signInManagerMock.Setup(um => um.PasswordSignInAsync(username, password, rememberMe, true)).ReturnsAsync(SignInResult.Success);

        _identityServerInteractionServiceMock.Setup(mock => mock.GetAuthorizationContextAsync(returnUrl)).ReturnsAsync(new AuthorizationRequest
        {
            Client = new Client { ClientId = clientId }
        });

        LoginModel model = new()
        {
            UserName = username,
            Password = password,
            Email = email,
            ReturnUrl = returnUrl,
            RememberMe = rememberMe
        };

        // Act
        LoginResult result = await _service.LoginAsync(model);

        // Assert
        Assert.Equal(returnUrl, result.ReturnUrl);
        Assert.Equal(username, result.UserName);
        Assert.Equal(Guid.Empty, result.UserId);

        AuthenticationProperties properties = (AuthenticationProperties)result.Properties;
        Assert.False(properties.IsPersistent);
        Assert.NotNull(properties.ExpiresUtc);
        _eventServiceMock.Verify(um => um.RaiseAsync(It.IsAny<UserLoginSuccessEvent>()), Times.Once());
    }

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Login_ShouldSetPersistentAndExpires()
    {
        // Arrange
        IdentitySettings settings = BuildIdentitySettings();
        settings.Login = new LoginSettings { AllowRememberLogin = true, RememberLoginDuration = 60 };
        InitMocks(settings);

        string email = "email@mail.com", username = "username", password = "password",
            returnUrl = "https:\\localhost:4200", clientId = "sfc";
        bool rememberMe = true;

        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser { UserName = username });

        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser { UserName = username });

        _signInManagerMock.Setup(um => um.PasswordSignInAsync(username, password, rememberMe, true)).ReturnsAsync(SignInResult.Success);

        _identityServerInteractionServiceMock.Setup(mock => mock.GetAuthorizationContextAsync(returnUrl)).ReturnsAsync(new AuthorizationRequest
        {
            Client = new Client { ClientId = clientId }
        });

        LoginModel model = new()
        {
            UserName = username,
            Password = password,
            Email = email,
            ReturnUrl = returnUrl,
            RememberMe = rememberMe
        };

        // Act
        LoginResult result = await _service.LoginAsync(model);

        // Assert
        AuthenticationProperties properties = (AuthenticationProperties)result.Properties;
        Assert.True(properties.IsPersistent);
        Assert.True(properties.ExpiresUtc > DateTimeOffset.UtcNow);
    }

    #endregion Login

    #region Logout

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Logout_ShouldProcessWithShowSignoutPrompt()
    {
        // Arrange
        string logoutId = "logout_id";
        IdentitySettings settings = BuildIdentitySettings();
        settings.Logout = new LogoutSettings { ShowLogoutPrompt = true };
        InitMocks(settings);

        _identityServerInteractionServiceMock.Setup(mock => mock.GetLogoutContextAsync(logoutId))
            .ReturnsAsync(new LogoutRequest(logoutId, new LogoutMessage()));

        LogoutModel model = new()
        {
            LogoutId = logoutId,
            User = new LogoutUserModel { IsAuthenticated = true }
        };

        // Act
        LogoutResult result = await _service.LogoutAsync(model);

        // Assert
        Assert.True(result.ShowLogoutPrompt);
        _eventServiceMock.Verify(um => um.RaiseAsync(It.IsAny<UserLogoutSuccessEvent>()), Times.Never());
        _signInManagerMock.Verify(um => um.SignOutAsync(), Times.Never());
    }

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Logout_ShouldProcessWithPostLogoutWhenNotShowLogoutPromptInSettings()
    {
        // Arrange
        string logoutId = "logout_id", postLogoutRedirectUri = "https:\\localhost:4200", clientName = "client_name",
            signOutIFrameUrl = "https:\\localhost:5200";
        IdentitySettings settings = BuildIdentitySettings();
        settings.Logout = new LogoutSettings { ShowLogoutPrompt = false, AutomaticRedirectAfterSignOut = true };
        InitMocks(settings);

        _identityServerInteractionServiceMock.Setup(mock => mock.GetLogoutContextAsync(logoutId))
            .ReturnsAsync(new LogoutRequest(signOutIFrameUrl, new LogoutMessage
            {
                PostLogoutRedirectUri = postLogoutRedirectUri,
                ClientName = clientName
            }));

        LogoutModel model = new()
        {
            LogoutId = logoutId,
            User = new LogoutUserModel { IsAuthenticated = true }
        };

        // Act
        LogoutResult result = await _service.LogoutAsync(model);

        // Assert
        Assert.False(result.ShowLogoutPrompt);
        Assert.True(result.AutomaticRedirectAfterSignOut);
        Assert.Equal(postLogoutRedirectUri, result.PostLogoutRedirectUrl);
        Assert.Equal(clientName, result.ClientName);
        Assert.Equal(signOutIFrameUrl, result.SignOutIFrameUrl);
        _eventServiceMock.Verify(um => um.RaiseAsync(It.IsAny<UserLogoutSuccessEvent>()), Times.Once());
        _signInManagerMock.Verify(um => um.SignOutAsync(), Times.Once());
    }

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_Logout_ShouldProcessWithPostLogoutWhenNotAuthenticated()
    {
        // Arrange
        string logoutId = "logout_id", postLogoutRedirectUri = "https:\\localhost:4200", clientName = "client_name",
            signOutIFrameUrl = "https:\\localhost:5200";
        IdentitySettings settings = BuildIdentitySettings();
        settings.Logout = new LogoutSettings { ShowLogoutPrompt = true };
        InitMocks(settings);

        _identityServerInteractionServiceMock.Setup(mock => mock.GetLogoutContextAsync(logoutId))
            .ReturnsAsync(new LogoutRequest(signOutIFrameUrl, new LogoutMessage
            {
                PostLogoutRedirectUri = postLogoutRedirectUri,
                ClientName = clientName
            }));

        LogoutModel model = new()
        {
            LogoutId = logoutId,
            User = new LogoutUserModel { IsAuthenticated = false }
        };

        // Act
        LogoutResult result = await _service.LogoutAsync(model);

        // Assert
        _eventServiceMock.Verify(um => um.RaiseAsync(It.IsAny<UserLogoutSuccessEvent>()), Times.Once());
        _signInManagerMock.Verify(um => um.SignOutAsync(), Times.Once());
    }

    #endregion Logout

    #region Post Logout

    [Fact]
    [Trait("Service", "Identity")]
    public async Task Service_Identity_PostLogout_ShouldReturnSuccessResult()
    {
        // Arrange
        string logoutId = "logout_id", postLogoutRedirectUri = "https:\\localhost:4200",
            clientId = "client_id", signOutIFrameUrl = "https:\\localhost:5200";
        IdentitySettings settings = BuildIdentitySettings();
        settings.Logout = new LogoutSettings { ShowLogoutPrompt = false, AutomaticRedirectAfterSignOut = true };
        InitMocks(settings);

        _identityServerInteractionServiceMock.Setup(mock => mock.GetLogoutContextAsync(logoutId))
            .ReturnsAsync(new LogoutRequest(signOutIFrameUrl, new LogoutMessage
            {
                PostLogoutRedirectUri = postLogoutRedirectUri,
                ClientId = clientId,
                ClientName = null
            }));

        LogoutModel model = new()
        {
            LogoutId = logoutId,
            User = new LogoutUserModel { IsAuthenticated = true }
        };

        // Act
        LogoutResult result = await _service.LogoutAsync(model);

        // Assert
        Assert.False(result.ShowLogoutPrompt);
        Assert.True(result.AutomaticRedirectAfterSignOut);
        Assert.Equal(postLogoutRedirectUri, result.PostLogoutRedirectUrl);
        Assert.Equal(clientId, result.ClientName);
        Assert.Equal(signOutIFrameUrl, result.SignOutIFrameUrl);
        _eventServiceMock.Verify(um => um.RaiseAsync(It.IsAny<UserLogoutSuccessEvent>()), Times.Once());
        _signInManagerMock.Verify(um => um.SignOutAsync(), Times.Once());
    }

    #endregion Post Logout

    #region Private

    private void InitMocks(IdentitySettings settings)
    {
        Mock<IUserStore<ApplicationUser>> userStore = new();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null!, null!, null!, null!);
        Mock<IOptions<IdentitySettings>> identitySettingsOptionsMock = new();
        identitySettingsOptionsMock.Setup(s => s.Value).Returns(settings);
        _identityServerInteractionServiceMock = new Mock<IIdentityServerInteractionService>();
        _serverUrlsMock = new Mock<IServerUrls>();
        _eventServiceMock = new Mock<IEventService>();
        _service = new IdentityService(_userManagerMock.Object, _signInManagerMock.Object,
            _identityServerInteractionServiceMock.Object, _serverUrlsMock.Object,
            _eventServiceMock.Object, identitySettingsOptionsMock.Object);
    }

    private static IdentitySettings BuildIdentitySettings()
    {
        return new()
        {
            Clients = [new ClientSetting { Id = "id" }],
            Api = new ApiSettings
            {
                Resources = [new ApiResourceSetting { Name = "sfc.data" }],
                Scopes = [new ApiScopeSetting { Name = "sfc.data.full" }]
            },
            Login = new LoginSettings(),
            Logout = new LogoutSettings()
        };
    }

    #endregion Private
}
