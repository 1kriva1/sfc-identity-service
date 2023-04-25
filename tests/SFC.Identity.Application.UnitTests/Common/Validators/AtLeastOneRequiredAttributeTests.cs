using SFC.Identity.Application.Common.Validators;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace SFC.Identity.Application.UnitTests.Common.Validators
{
    public class AtLeastOneRequiredAttributeTests
    {
        [Fact]
        public void Validator_ValidationUnknown_ShouldReturnFailedResultForFirstField()
        {
            // Arrange
            const string field1 = "Email", field2 = "UserName";

            AtLeastOneRequiredAttribute validator = new(field1, field2);

            var value = new { UserName = "username" };

            ValidationContext context = new(value);

            // Act
            ValidationResult? result = validator.GetValidationResult(value, context);

            // Assert
            Assert.Equal($"Unknown property: '{field1}'", result?.ErrorMessage);
        }

        [Fact]
        public void Validator_ValidationUnknown_ShouldReturnFailedResultForSecondField()
        {
            // Arrange
            const string field1 = "Email", field2 = "UserName";

            AtLeastOneRequiredAttribute validator = new(field1, field2);

            var value = new { Email = "email" };

            ValidationContext context = new(value);

            // Act
            ValidationResult? result = validator.GetValidationResult(value, context);

            // Assert
            Assert.Equal($"Unknown property: '{field2}'", result?.ErrorMessage);
        }

        [Fact]
        public void Validator_Validation_ShouldReturnSuccessResult()
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
        public void Validator_ValidationEmptyValues_ShouldReturnFailedResult()
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
            Assert.Equal($"Either or both of '{field1}' and '{field2}' are required.", result?.ErrorMessage);
        }

        [Fact]
        public void Validator_ValidationNullValues_ShouldReturnFailedResult()
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
            Assert.Equal($"Either or both of '{field1}' and '{field2}' are required.", result?.ErrorMessage);
        }
    }
}
