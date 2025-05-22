using System.ComponentModel.DataAnnotations;
using System.Reflection;

using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Extensions;

namespace SFC.Identity.Api.Infrastructure.Validators;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class AtLeastOneRequiredAttribute(string field1, string field2) : ValidationAttribute
{
    public string Field1 { get; } = field1;

    public string Field2 { get; } = field2;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage = Localization.AtLeastOneRequired.BuildValidationMessage(Field1, Field2);

        if (!TryGetProperty(Field1, validationContext, out PropertyInfo? property1))
        {
            return new ValidationResult(Localization.AtLeastOneRequiredUnknownProperty.BuildValidationMessage(Field1), [Field1]);
        }

        if (!TryGetProperty(Field2, validationContext, out PropertyInfo? property2))
        {
            return new ValidationResult(Localization.AtLeastOneRequiredUnknownProperty.BuildValidationMessage(Field2), [Field2]);
        }

        object? value1 = property1?.GetValue(validationContext.ObjectInstance);

        object? value2 = property2?.GetValue(validationContext.ObjectInstance);

        if (!string.IsNullOrEmpty(value1 as string) || !string.IsNullOrEmpty(value2 as string))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage, [Field1, Field2]);
    }

    private static bool TryGetProperty(string fieldName, ValidationContext validateionContext, out PropertyInfo? propertyInfo)
        => (propertyInfo = validateionContext.ObjectType.GetProperty(fieldName)) != null;


}