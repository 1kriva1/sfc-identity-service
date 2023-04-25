using SFC.Identity.Api.IntegrationTests.Fixtures;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Models;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.RefreshToken;
using SFC.Identity.Application.Models.Registration;
using SFC.Identity.Application.Models.Tokens;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace SFC.Identity.Api.IntegrationTests.Controllers
{
    public class IdentityControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public IdentityControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        #region Registration        

        [Fact]
        public async Task Identity_Register_ShouldReturnValidationErrorForPasswordExistence()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            RegistrationRequest request = new() { UserName = null, Email = null, Password = null!, ConfirmPassword = null! };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(RegistrationRequest.Password), new string[1] { "The Password field is required." } },
                { nameof(RegistrationRequest.ConfirmPassword), new string[1] { "The ConfirmPassword field is required." } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_Register_ShouldReturnValidationErrorForPasswordValue()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            RegistrationRequest request = new() { UserName = null, Email = null, Password = "pass", ConfirmPassword = "pass" };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> { { nameof(RegistrationRequest.Password), new string[1] { "Passwords must have at least 6 characters, " +
            "one non alphanumeric character, " +
            "one digit ('0'-'9'), " +
            "one uppercase ('A'-'Z'), " +
            "one lowercase ('a'-'z')." } } }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_Register_ShouldReturnValidationErrorForNotMatchedPasswords()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            RegistrationRequest request = new() { UserName = null, Email = null, Password = "Test1234!", ConfirmPassword = "Test12345!" };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(RegistrationRequest.ConfirmPassword), new string[1] { "'ConfirmPassword' and 'Password' do not match." } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_Register_ShouldReturnValidationErrorForInvalidUserName()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            RegistrationRequest request = new() { UserName = "Тест", Email = null, Password = "Test1234!", ConfirmPassword = "Test1234!" };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(RegistrationRequest.UserName), new string[1] { "The field UserName must match the regular expression '^[a-zA-Z0-9-._@+]+$'." } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_Register_ShouldReturnValidationErrorForInvalidEmail()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            RegistrationRequest request = new() { UserName = "Username", Email = "test_email", Password = "Test1234!", ConfirmPassword = "Test1234!" };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(RegistrationRequest.Email), new string[1] { "The Email field is not a valid e-mail address." } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_Register_ShouldReturnValidationErrorForMissingUserNameAndEmail()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            RegistrationRequest request = new() { UserName = null!, Email = null!, Password = "Test1234!", ConfirmPassword = "Test1234!" };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(RegistrationRequest.UserName), new string[1] { "Either or both of 'Email' and 'UserName' are required." } },
                { nameof(RegistrationRequest.Email), new string[1] { "Either or both of 'Email' and 'UserName' are required." } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_Register_ShouldReturnSuccessResult()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            RegistrationRequest request = new() { UserName = "Username_New", Email = "email_New@mail.com", Password = "Test1234!", ConfirmPassword = "Test1234!" };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            RegistrationResponse? responseValue = JsonSerializer.Deserialize<RegistrationResponse>(responseString);

            Assert.IsType<RegistrationResponse>(responseValue);
            Assert.True(responseValue.Success);
            Assert.Equal(CommonConstants.SUCCESS_MESSAGE, responseValue.Message);
            Assert.True(responseValue.UserId != Guid.Empty);
            Assert.NotNull(responseValue.Token);
        }

        #endregion Registration

        #region Login

        [Fact]
        public async Task Identity_Login_ShouldReturnValidationErrorForPasswordExistence()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            LoginRequest request = new() { UserName = null, Email = null, Password = null! };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(LoginRequest.Password), new string[1] { "The Password field is required." } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_Login_ShouldReturnValidationErrorForInvalidEmail()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            LoginRequest request = new() { UserName = "Username", Email = "test_email", Password = "Test1234!" };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(LoginRequest.Email), new string[1] { "The Email field is not a valid e-mail address." } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_Login_ShouldReturnValidationErrorForMissingUserNameAndEmail()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            LoginRequest request = new() { UserName = null!, Email = null!, Password = "Test1234!" };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(LoginRequest.UserName), new string[1] { "Either or both of 'Email' and 'UserName' are required." } },
                { nameof(LoginRequest.Email), new string[1] { "Either or both of 'Email' and 'UserName' are required." } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_Login_ShouldNotLoginUser()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            LoginRequest request = new() { UserName = "IntegrationTestUser", Email = "integrationtestemail@mail.com", Password = "wrong_pass" };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseResponse? responseValue = JsonSerializer.Deserialize<BaseResponse>(responseString);

            Assert.IsType<BaseResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal("User not found or incorrect password.", responseValue.Message);
        }

        [Fact]
        public async Task Identity_Login_ShouldLoginUser()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            LoginRequest request = new() { UserName = "IntegrationTestUser", Email = "integrationtestemail@mail.com", Password = "Test1234!" };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            LoginResponse? responseValue = JsonSerializer.Deserialize<LoginResponse>(responseString);

            Assert.IsType<LoginResponse>(responseValue);
            Assert.True(responseValue.Success);
            Assert.Equal(CommonConstants.SUCCESS_MESSAGE, responseValue.Message);
            Assert.Equal(Utilities.USER_ID, responseValue.UserId);
            Assert.NotNull(responseValue.Token);
        }

        #endregion Login

        #region Logout

        [Fact]
        public async Task Identity_Logout_ShouldReturnValidationErrorForUserIdExistence()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            LogoutRequest request = new() { UserId = null! };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/logout", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(LogoutRequest.UserId), new string[1] { "The UserId field is required." } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_Logout_ShouldNotLogoutUser()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            LogoutRequest request = new() { UserId = Guid.NewGuid().ToString() };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/logout", content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseResponse? responseValue = JsonSerializer.Deserialize<BaseResponse>(responseString);

            Assert.IsType<BaseResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal("User not found.", responseValue.Message);
        }

        [Fact]
        public async Task Identity_Logout_ShouldLogoutUser()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            LogoutRequest request = new() { UserId = Utilities.USER_ID.ToString() };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/logout", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            LogoutResponse? responseValue = JsonSerializer.Deserialize<LogoutResponse>(responseString);

            Assert.IsType<LogoutResponse>(responseValue);
            Assert.True(responseValue.Success);
            Assert.Equal(CommonConstants.SUCCESS_MESSAGE, responseValue.Message);
        }

        #endregion Logout

        #region RefreshToken

        [Fact]
        public async Task Identity_RefreshToken_ShouldReturnValidationErrorForTokenExistence()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            RefreshTokenRequest request = new() { Token = null! };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/refresh", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(RefreshTokenRequest.Token), new string[1] { "The Token field is required." } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_RefreshToken_ShouldNotRefreshToken()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            RefreshTokenRequest request = new()
            {
                Token = new JwtToken
                {
                    Access = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVGVzdFVzZXIiLCJleHAiOjE2ODIzNDI0MzksImlzcyI6Ikdsb2JvVGlja2V0SWRlbnRpdHkiLCJhdWQiOiJHbG9ib1RpY2tldElkZW50aXR5VXNlciJ9.EEV1kD8UseVlGxCMJJjkLyYKAtEwBrfjJmn2eGh43C0",
                    Refresh = "test_refresh"
                }
            };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/refresh", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

            Assert.IsType<BaseErrorResponse>(responseValue);
            Assert.False(responseValue.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, responseValue.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> {
                { nameof(RefreshTokenRequest.Token.Refresh), new string[1] { ErrorConstants.INVALID_TOKEN_ERROR_MESSAGE } }
            }, responseValue.Errors);
        }

        [Fact]
        public async Task Identity_RefreshToken_ShouldRefreshToken()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            RefreshTokenRequest request = new()
            {
                Token = new JwtToken
                {
                    Access = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSW50ZWdyYXRpb25UZXN0VXNlciIsImV4cCI6MTY4MjQwNTkxOCwiaXNzIjoiR2xvYm9UaWNrZXRJZGVudGl0eSIsImF1ZCI6Ikdsb2JvVGlja2V0SWRlbnRpdHlVc2VyIn0.sniULKxCa7wBI3OGqJMayaBdKo7zwPdo91XCt7cyleo",
                    Refresh = "kaGWK0agkRpoKjvRvnV+R/jswQJ1aqeszK9V61xN9vj0lD+yBj9EXFeM1GmWjChmtVeXWppJptak1uOOdpkBkA=="
                }
            };

            JsonContent content = JsonContent.Create(request);

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/identity/refresh", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();

            RefreshTokenResponse? responseValue = JsonSerializer.Deserialize<RefreshTokenResponse>(responseString);

            Assert.IsType<RefreshTokenResponse>(responseValue);
            Assert.True(responseValue.Success);
            Assert.Equal(CommonConstants.SUCCESS_MESSAGE, responseValue.Message);
            Assert.NotNull(responseValue.Token);
        }

        #endregion RefreshToken
    }
}
