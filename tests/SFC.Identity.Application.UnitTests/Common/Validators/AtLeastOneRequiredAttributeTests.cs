using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Validators;
using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.UnitTests.Common.Validators;

public class AtLeastOneRequiredAttributeTests
{
    [Fact]
    [Trait("Validator", "AtLeastOneRequired")]
    public void Validator_AtLeastOneRequired_ShouldReturnFailedResultForFirstField()
    {
        // Arrange
        const string field1 = "Email", field2 = "UserName";

        AtLeastOneRequiredAttribute validator = new(field1, field2);

        var value = new { UserName = "username" };

        ValidationContext context = new(value);

        // Act
        ValidationResult? result = validator.GetValidationResult(value, context);

        // Assert
        Assert.Equal(string.Format(Messages.AtLeastOneRequiredUnknownProperty, field1), result?.ErrorMessage);
    }

    [Fact]
    [Trait("Validator", "AtLeastOneRequired")]
    public void Validator_AtLeastOneRequired_ShouldReturnFailedResultForSecondField()
    {
        // Arrange
        const string field1 = "Email", field2 = "UserName";

        AtLeastOneRequiredAttribute validator = new(field1, field2);

        var value = new { Email = "email" };

        ValidationContext context = new(value);

        // Act
        ValidationResult? result = validator.GetValidationResult(value, context);

        // Assert
        Assert.Equal(string.Format(Messages.AtLeastOneRequiredUnknownProperty, field2), result?.ErrorMessage);
    }

    [Fact]
    [Trait("Validator", "AtLeastOneRequired")]
    public void Validator_AtLeastOneRequired_ShouldReturnSuccessResult()
    {
        // Arrange
        const string field1 = "Email", field2 = "UserName";

        AtLeastOneRequiredAttribute validator = new(field1, field2);

        var value = new { Email = "email", UserName = "username" };

        ValidationContext context = new(value);

        // Act
        ValidationResult? result = validator.GetValidationResult(value, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    [Trait("Validator", "AtLeastOneRequired")]
    public void Validator_AtLeastOneRequired_ShouldReturnFailedResultForEmptyValues()
    {
        // Arrange
        const string field1 = "Email", field2 = "UserName";

        AtLeastOneRequiredAttribute validator = new(field1, field2);

        var value = new { Email = string.Empty, UserName = string.Empty };

        ValidationContext context = new(value);

        // Act
        ValidationResult? result = validator.GetValidationResult(value, context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.MemberNames.Count());
        Assert.Contains(result.MemberNames, mm => mm.Equals(field1));
        Assert.Contains(result.MemberNames, mm => mm.Equals(field2));
        Assert.Equal(string.Format(Messages.AtLeastOneRequired, field1, field2), result?.ErrorMessage);
    }

    [Fact]
    [Trait("Validator", "AtLeastOneRequired")]
    public void Validator_AtLeastOneRequired_ShouldReturnFailedResultForNullValues()
    {
        // Arrange
        const string field1 = "Email", field2 = "UserName";

        AtLeastOneRequiredAttribute validator = new(field1, field2);

        var value = new { Email = null as string, UserName = null as string };

        ValidationContext context = new(value);

        // Act
        ValidationResult? result = validator.GetValidationResult(value, context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.MemberNames.Count());
        Assert.Contains(result.MemberNames, mm => mm.Equals(field1));
        Assert.Contains(result.MemberNames, mm => mm.Equals(field2));
        Assert.Equal(string.Format(Messages.AtLeastOneRequired, field1, field2), result?.ErrorMessage);
    }
}
