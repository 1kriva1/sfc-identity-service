using SFC.Identity.Application.Common.Exceptions;

using Xunit;

namespace SFC.Identity.Application.UnitTests.Common.Exceptions;
public class BadRequestExceptionTests
{
    [Fact]
    [Trait("Exception", "BadRequest")]
    public void Exception_BadRequest_ShouldHaveDefinedMessage()
    {
        // Arrange
        string validationMessage = "Test validation message.";

        // Act
        BadRequestException exception = new(validationMessage, []);

        // Assert
        Assert.Equal(validationMessage, exception.Message);
    }

    [Fact]
    [Trait("Exception", "BadRequest")]
    public void Exception_BadRequest_ShouldCreateDefinedErrors()
    {
        // Arrange
        string validationMessage = "Test validation message.";
        Dictionary<string, IEnumerable<string>> errors = new()
        {
            { "Key", new List<string>{ "Test error message."} }
        };

        // Act
        BadRequestException exception = new(validationMessage, errors);

        // Assert
        Assert.Equal(exception.Errors, errors);
    }

    [Fact]
    [Trait("Exception", "BadRequest")]
    public void Exception_BadRequest_ShouldCreateSingleError()
    {
        // Arrange
        string validationMessage = "Test validation message.";
        (string, string) singleError = ("Key", "Test error message.");

        // Act
        BadRequestException exception = new(validationMessage, singleError);

        // Assert
        Assert.True(exception.Errors.ContainsKey(singleError.Item1));
        Assert.Equal(singleError.Item2, exception.Errors[singleError.Item1].First());
    }
}
