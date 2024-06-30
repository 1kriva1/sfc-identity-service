using SFC.Identity.Application.Common.Exceptions;

using Xunit;

namespace SFC.Identity.Application.UnitTests.Common.Exceptions;
public class ConflictExceptionTests
{
    [Fact]
    [Trait("Exception", "Conflict")]
    public void Exception_Conflict_ShouldHaveDefinedMessage()
    {
        // Arrange
        string validationMessage = "Test validation message.";

        // Act
        ConflictException exception = new(validationMessage);

        // Assert
        Assert.Equal(validationMessage, exception.Message);
    }
}
