using System.Text.Json;

using Newtonsoft.Json.Linq;

namespace SFC.Identity.Application.UnitTests.Models.Base;
public class BaseErrorResponseTests
{
    //[Fact]
    //[Trait("Model", "BaseErrorResponse")]
    //public void Model_BaseErrorResponse_ShouldHaveDefinedMessageAndSuccessAndErrorsValues()
    //{
    //    // Arrange
    //    string message = "Test message";
    //    Dictionary<string, IEnumerable<string>> errors = new() { { "Key", new List<string> { "Test error message." } } };
    //    BaseErrorResponse response = new(message, errors);

    //    // Assert
    //    Assert.Equal(message, response.Message);
    //    Assert.False(response.Success);
    //    Assert.Equal(errors, response.Errors);
    //}

    //[Fact]
    //[Trait("Model", "BaseErrorResponse")]
    //public void Model_BaseErrorResponse_ShouldHaveCorrectPropertiesOrder()
    //{
    //    // Arrange
    //    BaseErrorResponse response = new();

    //    // Act
    //    string serializedBaseResponse = JsonSerializer.Serialize(response);
    //    JObject jsonObj = JObject.Parse(serializedBaseResponse);
    //    JProperty[] properties = jsonObj.Properties().ToArray();

    //    // Assert

    //    Assert.Equal(3, properties.Count());
    //    Assert.Equal(nameof(BaseErrorResponse.Success), properties[0].Name);
    //    Assert.Equal(nameof(BaseErrorResponse.Message), properties[1].Name);
    //    Assert.Equal(nameof(BaseErrorResponse.Errors), properties[2].Name);
    //}
}