using SFC.Identity.Application.Common.Exceptions;

using Xunit;

namespace SFC.Identity.Application.UnitTests.Common.Exceptions;
public class AuthorizationExceptionTests
{
    [Fact]
    [Trait("Exception", "Authorization")]
    public void Exception_Authorization_ShouldHaveDefinedMessage()
    {
        // Arrange
        string validationMessage = "Test validation message.";

        // Act
        AuthorizationException exception = new(validationMessage);

        // Assert
        Assert.Equal(validationMessage, exception.Message);
    }
}
