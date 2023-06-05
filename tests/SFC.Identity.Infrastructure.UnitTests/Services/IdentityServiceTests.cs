using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.RefreshToken;
using SFC.Identity.Application.Models.Registration;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Infrastructure.Persistence.Models;
using SFC.Identity.Infrastructure.Services;
using System.Security.Claims;
using Xunit;

namespace SFC.Identity.Infrastructure.UnitTests.Services
{
    public class IdentityServiceTests
    {
        private const string SUCCESS_MESSAGE = "Success result.";
        private const string VALIDATION_ERROR_MESSAGE = "Validation error.";
        private const string INVALID_TOKEN_ERROR_MESSAGE = "Invalid token.";

        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;

        private readonly Mock<IJwtService> _jwtServiceMock;

        private readonly JwtSettings _settings = new()
        {
            Key = "key_ahsvdjavsdvqwyvetyqweyvasndvhgavsdghcvahsdc",
            Issuer = "test_issuer",
            Audience = "test_audience",
            RefreshTokenDurationInDays = 7,
            AccessTokenDurationInMinutes = 2
        };

        private readonly IdentityService _service;

        public IdentityServiceTests()
        {
            Mock<IUserStore<ApplicationUser>> userStore = new();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);
            Mock<IOptions<JwtSettings>> jwtSettingsOptionsMock = new();
            jwtSettingsOptionsMock.Setup(s => s.Value).Returns(_settings);
            _jwtServiceMock = new Mock<IJwtService>();
            _service = new IdentityService(_userManagerMock.Object, _signInManagerMock.Object, _jwtServiceMock.Object);
        }

        #region Registration

        [Fact]
        [Trait("Identity", "Register")]
        public async Task IdentityService_Register_ShouldReturnConflictExceptionIfUserAlreadyExistByUserName()
        {
            // Arrange
            string username = "username";

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

            RegistrationRequest request = new()
            {
                UserName = username,
                Password = "password",
                ConfirmPassword = "password"
            };

            // Assert
            ConflictException exception = await Assert.ThrowsAsync<ConflictException>(async () => await _service.RegisterAsync(request));
            Assert.Equal("User already exists.", exception.Message);
        }

        [Fact]
        [Trait("Identity", "Register")]
        public async Task IdentityService_Register_ShouldReturnConflictExceptionIfUserAlreadyExistByEmail()
        {
            // Arrange
            string email = "email@mail.com";

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

            RegistrationRequest request = new()
            {
                Email = email,
                Password = "password",
                ConfirmPassword = "password"
            };

            // Assert
            ConflictException exception = await Assert.ThrowsAsync<ConflictException>(async () => await _service.RegisterAsync(request));
            Assert.Equal("User already exists.", exception.Message);
        }

        [Fact]
        [Trait("Identity", "Register")]
        public async Task IdentityService_Register_ShouldReturnIdentityExceptionIfProcessNotSuccess()
        {
            // Arrange
            string email = "email@mail.com", username = "username", errorCode = "100", errorDescription = "100 error description",
                password = "password";

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), password))
                .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError[1] {
                    new IdentityError { Code =errorCode, Description = errorDescription }
                })));

            RegistrationRequest request = new()
            {
                Email = "email_new@mail.com",
                UserName = "username_new",
                Password = password,
                ConfirmPassword = password
            };

            // Assert
            IdentityException exception = await Assert.ThrowsAsync<IdentityException>(async () => await _service.RegisterAsync(request));
            Assert.Equal("Error occurred during user registration process.", exception.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> { { errorCode, new string[1] { errorDescription } } }, exception.Errors);
        }

        [Fact]
        [Trait("Identity", "Register")]
        public async Task IdentityService_Register_ShouldReturnSuccessResponse()
        {
            // Arrange
            string email = "email@mail.com", username = "username", password = "password",
                claimType = "test_type", claimValue = "test_value",
                accessTokenValue = "test_access", refreshTokenValue = "test_refresh";

            Guid userId = Guid.NewGuid();

            List<Claim> claims = new() { new Claim(claimType, claimValue) };

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(claims);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), password))
                .Returns(Task.FromResult(IdentityResult.Success));

            _userManagerMock.Setup(mock => mock.UpdateAsync(It.IsAny<ApplicationUser>())).Callback<ApplicationUser>(user => user.Id = userId);

            _jwtServiceMock.Setup(jwt => jwt.GenerateAccessToken(claims)).Returns(new AccessToken { Value = accessTokenValue });

            _jwtServiceMock.Setup(jwt => jwt.GenerateRefreshToken()).Returns(new RefreshToken { Value = refreshTokenValue });

            RegistrationRequest request = new()
            {
                Email = "email_new@mail.com",
                UserName = "username_new",
                Password = password,
                ConfirmPassword = password
            };

            // Act
            RegistrationResponse response = await _service.RegisterAsync(request);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(SUCCESS_MESSAGE, response.Message);
            Assert.Equal(accessTokenValue, response.Token.Access);
            Assert.Equal(refreshTokenValue, response.Token.Refresh);
            Assert.Equal(userId, response.UserId);
            _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once());
        }

        #endregion Registration

        #region Login

        [Fact]
        [Trait("Identity", "Login")]
        public async Task IdentityService_Login_ShouldReturnAuthorizationExceptionIfUserNotFound()
        {
            // Arrange
            string username = "username", email = "email@mail.com";

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

            LoginRequest request = new()
            {
                UserName = "username_test",
                Password = "password",
                Email = "email_test@mail.com"
            };

            // Assert
            AuthorizationException exception = await Assert.ThrowsAsync<AuthorizationException>(async () => await _service.LoginAsync(request));
            Assert.Equal("User not found or incorrect password.", exception.Message);
        }

        [Fact]
        [Trait("Identity", "Login")]
        public async Task IdentityService_Login_ShouldReturnAuthorizationExceptionIfLoginFailed()
        {
            // Arrange
            string username = "username", email = "email@mail.com", password = "password";

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

            _signInManagerMock.Setup(um => um.PasswordSignInAsync(username, password, false, true)).ReturnsAsync(SignInResult.Failed);

            LoginRequest request = new()
            {
                UserName = username,
                Password = password,
                Email = email
            };

            // Assert
            AuthorizationException exception = await Assert.ThrowsAsync<AuthorizationException>(async () => await _service.LoginAsync(request));
            Assert.Equal("User not found or incorrect password.", exception.Message);
        }

        [Fact]
        [Trait("Identity", "Login")]
        public async Task IdentityService_Login_ShouldReturnForbiddenExceptionIfUserLockedIout()
        {
            // Arrange
            string username = "username", email = "email@mail.com", password = "password";

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

            _signInManagerMock.Setup(um => um.PasswordSignInAsync(username, password, false, true)).ReturnsAsync(SignInResult.LockedOut);

            LoginRequest request = new()
            {
                UserName = username,
                Password = password,
                Email = email
            };

            // Assert
            ForbiddenException exception = await Assert.ThrowsAsync<ForbiddenException>(async () => await _service.LoginAsync(request));
            Assert.Equal("User account locked out.", exception.Message);
        }

        [Fact]
        [Trait("Identity", "Login")]
        public async Task IdentityService_Login_ShouldReturnSuccessResponse()
        {
            // Arrange
            string email = "email@mail.com", username = "username", password = "password",
               claimType = "test_type", claimValue = "test_value",
               accessTokenValue = "test_access", refreshTokenValue = "test_refresh";

            Guid userId = Guid.NewGuid();

            List<Claim> claims = new() { new Claim(claimType, claimValue) };

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(claims);

            _userManagerMock.Setup(mock => mock.UpdateAsync(It.IsAny<ApplicationUser>())).Callback<ApplicationUser>(user => user.Id = userId);

            _jwtServiceMock.Setup(jwt => jwt.GenerateAccessToken(claims)).Returns(new AccessToken { Value = accessTokenValue });

            _jwtServiceMock.Setup(jwt => jwt.GenerateRefreshToken()).Returns(new RefreshToken { Value = refreshTokenValue });

            _signInManagerMock.Setup(um => um.PasswordSignInAsync(username, password, false, true)).ReturnsAsync(SignInResult.Success);

            LoginRequest request = new()
            {
                UserName = username,
                Password = password,
                Email = email
            };

            // Act
            LoginResponse response = await _service.LoginAsync(request);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(SUCCESS_MESSAGE, response.Message);
            Assert.Equal(accessTokenValue, response.Token.Access);
            Assert.Equal(refreshTokenValue, response.Token.Refresh);
            Assert.Equal(userId, response.UserId);
            _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once());
        }

        #endregion Login

        #region Refresh token

        [Fact]
        [Trait("Identity", "RefreshToken")]
        public async Task IdentityService_RefreshToken_ShouldReturnBadRequestExceptionIfPrincipalNotFound()
        {
            // Arrange
            string accessTokenValue = "test_access";

            _jwtServiceMock.Setup(jwt => jwt.GetPrincipalFromExpiredToken(accessTokenValue)).Returns(null as ClaimsPrincipal);

            RefreshTokenRequest request = new()
            {
                Token = new JwtToken
                {
                    Access = accessTokenValue
                }
            };

            // Assert
            BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(async () => await _service.RefreshTokenAsync(request));
            Assert.Equal(VALIDATION_ERROR_MESSAGE, exception.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> { { nameof(request.Token.Access), new string[1] { INVALID_TOKEN_ERROR_MESSAGE } } }, exception.Errors);
        }

        [Fact]
        [Trait("Identity", "RefreshToken")]
        public async Task IdentityService_RefreshToken_ShouldReturnAuthorizationExceptionIfUserNotFound()
        {
            // Arrange
            string accessTokenValue = "test_access";

            _jwtServiceMock.Setup(jwt => jwt.GetPrincipalFromExpiredToken(accessTokenValue)).Returns(new ClaimsPrincipal());

            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(null as ApplicationUser);

            RefreshTokenRequest request = new()
            {
                Token = new JwtToken
                {
                    Access = accessTokenValue
                }
            };

            // Assert
            AuthorizationException exception = await Assert.ThrowsAsync<AuthorizationException>(async () => await _service.RefreshTokenAsync(request));
            Assert.Equal("User not found or incorrect token.", exception.Message);
        }

        [Fact]
        [Trait("Identity", "RefreshToken")]
        public async Task IdentityService_RefreshToken_ShouldReturnBadRequestExceptionIfUserHasNotAccessToken()
        {
            // Arrange
            string accessTokenValue = "test_access";

            _jwtServiceMock.Setup(jwt => jwt.GetPrincipalFromExpiredToken(accessTokenValue)).Returns(new ClaimsPrincipal());

            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { AccessToken = null });

            RefreshTokenRequest request = new()
            {
                Token = new JwtToken
                {
                    Access = accessTokenValue
                }
            };

            // Assert
            BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(async () => await _service.RefreshTokenAsync(request));
            Assert.Equal(VALIDATION_ERROR_MESSAGE, exception.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> { { nameof(request.Token.Refresh), new string[1] { INVALID_TOKEN_ERROR_MESSAGE } } }, exception.Errors);
        }

        [Fact]
        [Trait("Identity", "RefreshToken")]
        public async Task IdentityService_RefreshToken_ShouldReturnBadRequestExceptionIfUsersRefreshTokenNotEqualToRequestRefreshToken()
        {
            // Arrange
            string accessTokenValue = "test_access", refreshTokenValue = "test_refresh";

            _jwtServiceMock.Setup(jwt => jwt.GetPrincipalFromExpiredToken(accessTokenValue)).Returns(new ClaimsPrincipal());

            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser
            {
                AccessToken = new AccessToken { RefreshToken = new RefreshToken { Value = "another_refresh" } }
            });

            RefreshTokenRequest request = new()
            {
                Token = new JwtToken
                {
                    Access = accessTokenValue,
                    Refresh = refreshTokenValue
                }
            };

            // Assert
            BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(async () => await _service.RefreshTokenAsync(request));
            Assert.Equal(VALIDATION_ERROR_MESSAGE, exception.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> { { nameof(request.Token.Refresh), new string[1] { INVALID_TOKEN_ERROR_MESSAGE } } }, exception.Errors);
        }

        [Fact]
        [Trait("Identity", "RefreshToken")]
        public async Task IdentityService_RefreshToken_ShouldReturnBadRequestExceptionIfUsersRefreshTokenExpired()
        {
            // Arrange
            string accessTokenValue = "test_access", refreshTokenValue = "test_refresh";

            _jwtServiceMock.Setup(jwt => jwt.GetPrincipalFromExpiredToken(accessTokenValue)).Returns(new ClaimsPrincipal());

            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser
            {
                AccessToken = new AccessToken { RefreshToken = new RefreshToken { Value = refreshTokenValue, ExpiresDate = DateTime.MinValue } }
            });

            RefreshTokenRequest request = new()
            {
                Token = new JwtToken
                {
                    Access = accessTokenValue,
                    Refresh = refreshTokenValue
                }
            };

            // Assert
            BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(async () => await _service.RefreshTokenAsync(request));
            Assert.Equal(VALIDATION_ERROR_MESSAGE, exception.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> { { nameof(request.Token.Refresh), new string[1] { INVALID_TOKEN_ERROR_MESSAGE } } }, exception.Errors);
        }

        [Fact]
        [Trait("Identity", "RefreshToken")]
        public async Task IdentityService_RefreshToken_ShouldReturnSuccessResponse()
        {
            // Arrange
            string accessTokenValue = "test_access", refreshTokenValue = "test_refresh",
                 claimType = "test_type", claimValue = "test_value";

            List<Claim> claims = new() { new Claim(claimType, claimValue) };

            _jwtServiceMock.Setup(jwt => jwt.GetPrincipalFromExpiredToken(accessTokenValue)).Returns(new ClaimsPrincipal());

            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser
            {
                AccessToken = new AccessToken { RefreshToken = new RefreshToken { Value = refreshTokenValue, ExpiresDate = DateTime.MaxValue } }
            });

            _userManagerMock.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(claims);

            _jwtServiceMock.Setup(jwt => jwt.GenerateAccessToken(claims)).Returns(new AccessToken { Value = accessTokenValue });

            _jwtServiceMock.Setup(jwt => jwt.GenerateRefreshToken()).Returns(new RefreshToken { Value = refreshTokenValue });

            RefreshTokenRequest request = new()
            {
                Token = new JwtToken
                {
                    Access = accessTokenValue,
                    Refresh = refreshTokenValue
                }
            };

            // Act
            RefreshTokenResponse response = await _service.RefreshTokenAsync(request);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(SUCCESS_MESSAGE, response.Message);
            Assert.Equal(accessTokenValue, response.Token.Access);
            Assert.Equal(refreshTokenValue, response.Token.Refresh);
            _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once());
        }

        #endregion Refresh token

        #region Logout

        [Fact]
        [Trait("Identity", "Logout")]
        public async Task IdentityService_Logout_ShouldReturnNotFoundExceptionIfUserNotFound()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();

            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(new ApplicationUser());

            LogoutRequest request = new()
            {
                UserId = Guid.NewGuid().ToString()
            };

            // Assert
            NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.LogoutAsync(request));
            Assert.Equal("User not found.", exception.Message);
        }

        [Fact]
        [Trait("Identity", "Logout")]
        public async Task IdentityService_Logout_ShouldReturnSuccessResult()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ApplicationUser user = new ApplicationUser { AccessToken = new AccessToken() };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);

            LogoutRequest request = new()
            {
                UserId = userId
            };

            // Act
            LogoutResponse response = await _service.LogoutAsync(request);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(SUCCESS_MESSAGE, response.Message);
            Assert.Null(user.AccessToken);
            _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once());
            _signInManagerMock.Verify(um => um.SignOutAsync(), Times.Once());
            
        }

        #endregion Logout
    }
}
