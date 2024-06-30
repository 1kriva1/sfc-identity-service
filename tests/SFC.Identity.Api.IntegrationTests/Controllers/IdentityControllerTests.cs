using SFC.Identity.Api.IntegrationTests.Fixtures;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Models.Base;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.Registration;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Xunit;

namespace SFC.Identity.Api.IntegrationTests.Controllers;

public class IdentityControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public IdentityControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _factory.InitializeDbForTests();
    }

    #region Registration        

    [Fact]
    [Trait("Identity", "Register")]
    public async Task Identity_Register_ShouldReturnValidationErrorForPasswordExistence()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        RegistrationRequest request = new() { UserName = null, Email = null, Password = null!, ConfirmPassword = null! };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(nameof(RegistrationRequest.ConfirmPassword)));
        Assert.Single(responseValue.Errors[nameof(RegistrationRequest.ConfirmPassword)]);
        Assert.Equal("The ConfirmPassword field is required.", responseValue.Errors[nameof(RegistrationRequest.ConfirmPassword)].FirstOrDefault());
        Assert.True(responseValue.Errors.ContainsKey(nameof(RegistrationRequest.Password)));
        Assert.Single(responseValue.Errors[nameof(RegistrationRequest.Password)]);
        Assert.Equal("The Password field is required.", responseValue.Errors[nameof(RegistrationRequest.Password)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Register")]
    public async Task Identity_Register_ShouldReturnValidationErrorForPasswordValue()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        RegistrationRequest request = new() { UserName = null, Email = null, Password = "pass", ConfirmPassword = "pass" };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(nameof(RegistrationRequest.Password)));
        Assert.Single(responseValue.Errors[nameof(RegistrationRequest.Password)]);
        Assert.Equal("Passwords must have at least 6 characters, " +
        "one non alphanumeric character, " +
        "one digit ('0'-'9'), " +
        "one uppercase ('A'-'Z'), " +
        "one lowercase ('a'-'z').", responseValue.Errors[nameof(RegistrationRequest.Password)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Register")]
    public async Task Identity_Register_ShouldReturnValidationErrorForNotMatchedPasswords()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        RegistrationRequest request = new() { UserName = null, Email = null, Password = "Test1234!", ConfirmPassword = "Test12345!" };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(nameof(RegistrationRequest.ConfirmPassword)));
        Assert.Single(responseValue.Errors[nameof(RegistrationRequest.ConfirmPassword)]);
        Assert.Equal("'Password' and 'Confirm password' do not match.", responseValue.Errors[nameof(RegistrationRequest.ConfirmPassword)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Register")]
    public async Task Identity_Register_ShouldReturnValidationErrorForInvalidUserName()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        RegistrationRequest request = new() { UserName = "Тест", Email = null, Password = "Test1234!", ConfirmPassword = "Test1234!" };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(nameof(RegistrationRequest.UserName)));
        Assert.Single(responseValue.Errors[nameof(RegistrationRequest.UserName)]);
        Assert.Equal("User name can only have letters, numbers and special characters -._@+.", responseValue.Errors[nameof(RegistrationRequest.UserName)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Register")]
    public async Task Identity_Register_ShouldReturnValidationErrorForInvalidEmail()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        RegistrationRequest request = new() { UserName = "Username", Email = "test_email", Password = "Test1234!", ConfirmPassword = "Test1234!" };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(nameof(RegistrationRequest.Email)));
        Assert.Single(responseValue.Errors[nameof(RegistrationRequest.Email)]);
        Assert.Equal("The Email field is not a valid e-mail address.", responseValue.Errors[nameof(RegistrationRequest.Email)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Register")]
    public async Task Identity_Register_ShouldReturnLocalizedValidationErrorForInvalidEmail()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add(Constants.ACCEPT_LANGUAGE, CommonConstants.SUPPORTED_CULTURES[1]);

        RegistrationRequest request = new() { UserName = "Username", Email = "test_email", Password = "Test1234!", ConfirmPassword = "Test1234!" };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.Equal("Валідаційна помилка.", responseValue!.Message);
        Assert.Equal("Поле електронної пошти не є дійсною адресою електронної пошти.", responseValue!.Errors![nameof(RegistrationRequest.Email)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Register")]
    public async Task Identity_Register_ShouldReturnValidationErrorForMissingUserNameAndEmail()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        RegistrationRequest request = new() { UserName = null!, Email = null!, Password = "Test1234!", ConfirmPassword = "Test1234!" };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(nameof(RegistrationRequest.UserName)));
        Assert.Single(responseValue.Errors[nameof(RegistrationRequest.UserName)]);
        Assert.Equal(string.Format(Messages.AtLeastOneRequired, nameof(RegistrationRequest.Email), nameof(RegistrationRequest.UserName)),
            responseValue.Errors[nameof(RegistrationRequest.UserName)].FirstOrDefault());
        Assert.True(responseValue.Errors.ContainsKey(nameof(RegistrationRequest.Email)));
        Assert.Single(responseValue.Errors[nameof(RegistrationRequest.Email)]);
        Assert.Equal(string.Format(Messages.AtLeastOneRequired, nameof(RegistrationRequest.Email), nameof(RegistrationRequest.UserName)),
            responseValue.Errors[nameof(RegistrationRequest.Email)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Register")]
    public async Task Identity_Register_ShouldReturnSuccessResult()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        RegistrationRequest request = new() { UserName = "Username_New", Email = "email_New@mail.com", Password = "Test1234!", ConfirmPassword = "Test1234!" };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        RegistrationResponse? responseValue = JsonSerializer.Deserialize<RegistrationResponse>(responseString);

        Assert.IsType<RegistrationResponse>(responseValue);
        Assert.True(responseValue.Success);
        Assert.Equal(Messages.SuccessResult, responseValue.Message);
        Assert.NotEmpty(responseValue.ReturnUrl);
    }

    #endregion Registration

    #region Login

    [Fact]
    [Trait("Identity", "Login")]
    public async Task Identity_Login_ShouldReturnValidationErrorForPasswordExistence()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        LoginRequest request = new() { UserName = null, Email = null, Password = null! };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(nameof(LoginRequest.Password)));
        Assert.Single(responseValue.Errors[nameof(LoginRequest.Password)]);
        Assert.Equal("The Password field is required.", responseValue.Errors[nameof(LoginRequest.Password)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Login")]
    public async Task Identity_Login_ShouldReturnLocalizedValidationErrorForPasswordExistence()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add(Constants.ACCEPT_LANGUAGE, CommonConstants.SUPPORTED_CULTURES[1]);

        LoginRequest request = new() { UserName = null, Email = null, Password = null! };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.Equal("Валідаційна помилка.", responseValue!.Message);
        Assert.Equal("Поле Пароль є обов’язковим для заповнення.", responseValue!.Errors![nameof(LoginRequest.Password)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Login")]
    public async Task Identity_Login_ShouldReturnValidationErrorForInvalidEmail()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        LoginRequest request = new() { UserName = "Username", Email = "test_email", Password = "Test1234!" };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(nameof(LoginRequest.Email)));
        Assert.Single(responseValue.Errors[nameof(LoginRequest.Email)]);
        Assert.Equal("The Email field is not a valid e-mail address.", responseValue.Errors[nameof(LoginRequest.Email)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Login")]
    public async Task Identity_Login_ShouldReturnValidationErrorForMissingUserNameAndEmail()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        LoginRequest request = new() { UserName = null!, Email = null!, Password = "Test1234!" };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(nameof(LoginRequest.Email)));
        Assert.Single(responseValue.Errors[nameof(LoginRequest.Email)]);
        Assert.Equal(string.Format(Messages.AtLeastOneRequired, nameof(RegistrationRequest.Email), nameof(RegistrationRequest.UserName)),
            responseValue.Errors[nameof(LoginRequest.Email)].FirstOrDefault());
        Assert.True(responseValue.Errors.ContainsKey(nameof(LoginRequest.UserName)));
        Assert.Single(responseValue.Errors[nameof(LoginRequest.UserName)]);
        Assert.Equal(string.Format(Messages.AtLeastOneRequired, nameof(RegistrationRequest.Email), nameof(RegistrationRequest.UserName)),
            responseValue.Errors[nameof(LoginRequest.UserName)].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Login")]
    public async Task Identity_Login_ShouldNotLoginUser()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        LoginRequest request = new() { UserName = "IntegrationTestUser", Email = "integrationtestemail@mail.com", Password = "wrong_pass" };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseResponse? responseValue = JsonSerializer.Deserialize<BaseResponse>(responseString);

        Assert.IsType<BaseResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.AuthorizationError, responseValue.Message);
    }

    [Fact]
    [Trait("Identity", "Login")]
    public async Task Identity_Login_ShouldLoginUser()
    {
        // Act
        LoginResponse? responseValue = await LoginPlayer();

        Assert.IsType<LoginResponse>(responseValue);
        Assert.True(responseValue.Success);
        Assert.Equal(Messages.SuccessResult, responseValue.Message);
        Assert.NotEmpty(responseValue.ReturnUrl);
    }

    #endregion Login

    #region Logout

    [Fact]
    [Trait("Identity", "Logout")]
    public async Task Identity_Logout_ShouldReturnBadRequest()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync($"{Constants.API_IDENTITY}/logout");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(Constants.LOGOUT_ID_QUERY_PARAMETER_NAME));
        Assert.Single(responseValue.Errors[Constants.LOGOUT_ID_QUERY_PARAMETER_NAME]);
        Assert.Equal($"The {Constants.LOGOUT_ID_QUERY_PARAMETER_NAME} field is required.",
            responseValue.Errors[Constants.LOGOUT_ID_QUERY_PARAMETER_NAME].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Logout")]
    public async Task Identity_Logout_ShouldReturnLocalizedValidationErrorForMissingLogoutId()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(Constants.ACCEPT_LANGUAGE, CommonConstants.SUPPORTED_CULTURES[1]);

        // Act
        HttpResponseMessage response = await client.GetAsync($"{Constants.API_IDENTITY}/logout");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.Equal("Валідаційна помилка.", responseValue!.Message);
        Assert.Equal($"Поле {Constants.LOGOUT_ID_QUERY_PARAMETER_NAME} обов’язкове.",
            responseValue?.Errors![Constants.LOGOUT_ID_QUERY_PARAMETER_NAME].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "Logout")]
    public async Task Identity_Logout_ShouldLogoutUser()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        string logoutId = "logout_id";

        // Act
        HttpResponseMessage response = await client.GetAsync($"{Constants.API_IDENTITY}/logout?{Constants.LOGOUT_ID_QUERY_PARAMETER_NAME}={logoutId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        LogoutResponse? responseValue = JsonSerializer.Deserialize<LogoutResponse>(responseString);

        Assert.IsType<LogoutResponse>(responseValue);
        Assert.True(responseValue.Success);
        Assert.Equal(Messages.SuccessResult, responseValue.Message);
        Assert.Null(responseValue?.SignOutIFrameUrl);
        Assert.Null(responseValue?.ClientName);
        Assert.Null(responseValue?.PostLogoutRedirectUrl);
        Assert.True(responseValue?.AutomaticRedirectAfterSignOut);
        Assert.False(responseValue?.ShowLogoutPrompt);
    }

    #endregion Logout

    #region Post Logout

    [Fact]
    [Trait("Identity", "PostLogout")]
    public async Task Identity_PostLogout_ShouldReturnBadRequest()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/logout", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.IsType<BaseErrorResponse>(responseValue);
        Assert.False(responseValue.Success);
        Assert.Equal(Messages.ValidationError, responseValue.Message);
        Assert.NotNull(responseValue.Errors);
        Assert.True(responseValue.Errors.ContainsKey(Constants.LOGOUT_ID_QUERY_PARAMETER_NAME));
        Assert.Single(responseValue.Errors[Constants.LOGOUT_ID_QUERY_PARAMETER_NAME]);
        Assert.Equal($"The {Constants.LOGOUT_ID_QUERY_PARAMETER_NAME} field is required.",
            responseValue.Errors[Constants.LOGOUT_ID_QUERY_PARAMETER_NAME].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "PostLogout")]
    public async Task Identity_PostLogout_ShouldReturnLocalizedValidationErrorForMissingLogoutId()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(Constants.ACCEPT_LANGUAGE, CommonConstants.SUPPORTED_CULTURES[1]);

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/logout", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        BaseErrorResponse? responseValue = JsonSerializer.Deserialize<BaseErrorResponse>(responseString);

        Assert.Equal("Валідаційна помилка.", responseValue!.Message);
        Assert.Equal($"Поле {Constants.LOGOUT_ID_QUERY_PARAMETER_NAME} обов’язкове.",
            responseValue?.Errors![Constants.LOGOUT_ID_QUERY_PARAMETER_NAME].FirstOrDefault());
    }

    [Fact]
    [Trait("Identity", "PostLogout")]
    public async Task Identity_PostLogout_ShouldLogoutUser()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        string logoutId = "logout_id";

        // Act
        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/logout?{Constants.LOGOUT_ID_QUERY_PARAMETER_NAME}={logoutId}", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        LogoutResponse? responseValue = JsonSerializer.Deserialize<LogoutResponse>(responseString);

        Assert.IsType<LogoutResponse>(responseValue);
        Assert.True(responseValue.Success);
        Assert.Equal(Messages.SuccessResult, responseValue.Message);
        Assert.Null(responseValue?.SignOutIFrameUrl);
        Assert.Null(responseValue?.ClientName);
        Assert.Null(responseValue?.PostLogoutRedirectUrl);
        Assert.True(responseValue?.AutomaticRedirectAfterSignOut);
        Assert.False(responseValue?.ShowLogoutPrompt);
    }

    #endregion Post Logout

    #region Private

    private async Task<LoginResponse?> LoginPlayer()
    {
        HttpClient client = _factory.CreateClient();

        LoginRequest request = new() { UserName = "IntegrationTestUser", Email = "integrationtestemail@mail.com", Password = "Test1234!" };

        JsonContent content = JsonContent.Create(request);

        HttpResponseMessage response = await client.PostAsync($"{Constants.API_IDENTITY}/login", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        LoginResponse? responseValue = JsonSerializer.Deserialize<LoginResponse>(responseString);

        return responseValue;
    }

    #endregion Private
}
