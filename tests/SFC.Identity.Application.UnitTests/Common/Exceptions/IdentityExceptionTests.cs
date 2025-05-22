//using SFC.Identity.Application.Common.Exceptions;

//namespace SFC.Identity.Application.UnitTests.Common.Exceptions;
//public class IdentityExceptionTests
//{
//    [Fact]
//    [Trait("Exception", "Identity")]
//    public void Exception_Identity_ShouldHaveDefinedMessage()
//    {
//        // Arrange
//        string validationMessage = "Test validation message.";

//        // Act
//        IdentityException exception = new(validationMessage, []);

//        // Assert
//        Assert.Equal(validationMessage, exception.Message);
//    }

//    [Fact]
//    [Trait("Exception", "Identity")]
//    public void Exception_Identity_ShouldCreateDefinedErrors()
//    {
//        // Arrange
//        string validationMessage = "Test validation message.";
//        Dictionary<string, IEnumerable<string>> errors = new()
//        {
//            { "Key", new List<string>{ "Test error message."} }
//        };

//        // Act
//        IdentityException exception = new(validationMessage, errors);

//        // Assert
//        Assert.Equal(exception.Errors, errors);
//    }
//}