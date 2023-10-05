using SFC.Identity.Api.IntegrationTests.Fixtures;
using System.Net;
using Xunit;
using System.Text.Json;
using SFC.Identity.Application.Models.Existence;
using SFC.Identity.Application.Common.Constants;

namespace SFC.Identity.Api.IntegrationTests.Controllers;

public class ExistenceControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ExistenceControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _factory.InitializeDbForTests();
    }

    [Fact]
    [Trait("API", "Integration")]
    public async Task API_Integration_Existence_CheckByUserName_ShouldReturnUserExist()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        string userName = "IntegrationTestUser";

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/existence/name/{userName}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        ExistenceResponse? responseValue = JsonSerializer.Deserialize<ExistenceResponse>(responseString);

        Assert.IsType<ExistenceResponse>(responseValue);
        Assert.True(responseValue.Success);
        Assert.Equal(Messages.SuccessResult, responseValue.Message);
        Assert.True(responseValue.Exist);
    }

    [Fact]
    [Trait("API", "Integration")]
    public async Task API_Integration_Existence_CheckByUserName_ShouldReturnUserNotExist()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        string userName = "IntegrationTestUser_Another";

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/existence/name/{userName}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        ExistenceResponse? responseValue = JsonSerializer.Deserialize<ExistenceResponse>(responseString);

        Assert.IsType<ExistenceResponse>(responseValue);
        Assert.True(responseValue.Success);
        Assert.Equal(Messages.SuccessResult, responseValue.Message);
        Assert.False(responseValue.Exist);
    }

    [Fact]
    [Trait("API", "Integration")]
    public async Task API_Integration_Existence_CheckByUserName_ShouldReturnLocalizedMessage()
    {
        // Arrange
        // Arrange
        HttpClient client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add("Accept-Language", CommonConstants.SUPPORTED_CULTURES[1]);

        string userName = "IntegrationTestUser_Another";

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/existence/name/{userName}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        Assert.Contains("Успішний результат.", responseString);
    }

    [Fact]
    [Trait("API", "Integration")]
    public async Task API_Integration_Existence_CheckByEmail_ShouldReturnUserExist()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        string email = "integrationtestemail@mail.com";

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/existence/email/{email}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        ExistenceResponse? responseValue = JsonSerializer.Deserialize<ExistenceResponse>(responseString);

        Assert.IsType<ExistenceResponse>(responseValue);
        Assert.True(responseValue.Success);
        Assert.Equal(Messages.SuccessResult, responseValue.Message);
        Assert.True(responseValue.Exist);
    }

    [Fact]
    [Trait("API", "Integration")]
    public async Task API_Integration_Existence_CheckByEmail_ShouldReturnUserNotExist()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        string email = "testemail_another@mail.com";

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/existence/email/{email}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        ExistenceResponse? responseValue = JsonSerializer.Deserialize<ExistenceResponse>(responseString);

        Assert.IsType<ExistenceResponse>(responseValue);
        Assert.True(responseValue.Success);
        Assert.Equal(Messages.SuccessResult, responseValue.Message);
        Assert.False(responseValue.Exist);
    }

    [Fact]
    [Trait("API", "Integration")]
    public async Task API_Integration_Existence_CheckByEmail_ShouldReturnLocalizedMessage()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add("Accept-Language", CommonConstants.SUPPORTED_CULTURES[1]);

        string email = "integrationtestemail@mail.com";

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/existence/email/{email}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string responseString = await response.Content.ReadAsStringAsync();

        Assert.Contains("Успішний результат.", responseString);
    }
}
