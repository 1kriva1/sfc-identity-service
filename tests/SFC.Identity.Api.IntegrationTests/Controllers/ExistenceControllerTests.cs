using SFC.Identity.Api.IntegrationTests.Fixtures;
using System.Net;
using Xunit;
using System.Text.Json;
using SFC.Identity.Application.Models.Existence;

namespace SFC.Identity.Api.IntegrationTests.Controllers
{
    public class ExistenceControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private const string SUCCESS_MESSAGE = "Success result.";

        private readonly CustomWebApplicationFactory<Program> _factory;

        public ExistenceControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _factory.InitializeDbForTests();
        }

        [Fact]
        [Trait("Existence", "CheckByUserName")]
        public async Task Existence_CheckByUserName_ShouldReturnUserExist()
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
            Assert.Equal(SUCCESS_MESSAGE, responseValue.Message);
            Assert.True(responseValue.Exist);
        }

        [Fact]
        [Trait("Existence", "CheckByUserName")]
        public async Task Existence_CheckByUserName_ShouldReturnUserNotExist()
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
            Assert.Equal(SUCCESS_MESSAGE, responseValue.Message);
            Assert.False(responseValue.Exist);
        }

        [Fact]
        [Trait("Existence", "CheckByEmail")]
        public async Task Existence_CheckByEmail_ShouldReturnUserExist()
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
            Assert.Equal(SUCCESS_MESSAGE, responseValue.Message);
            Assert.True(responseValue.Exist);
        }

        [Fact]
        [Trait("Existence", "CheckByEmail")]
        public async Task Existence_CheckByEmail_ShouldReturnUserNotExist()
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
            Assert.Equal(SUCCESS_MESSAGE, responseValue.Message);
            Assert.False(responseValue.Exist);
        }
    }
}
