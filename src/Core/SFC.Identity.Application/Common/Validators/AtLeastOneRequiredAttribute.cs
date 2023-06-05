using SFC.Identity.Application.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SFC.Identity.Application.Common.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AtLeastOneRequiredAttribute : ValidationAttribute
    {
        private readonly string _field1;

        private readonly string _field2;

        public AtLeastOneRequiredAttribute(string field1, string field2)
            => (_field1, _field2) = (field1, field2);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            ErrorMessage = string.Format(Messages.AtLeastOneRequired, _field1, _field2);

            if (!TryGetProperty(_field1, validationContext, out PropertyInfo? property1))
            {
                return new ValidationResult(string.Format(Messages.AtLeastOneRequiredUnknownProperty, _field1), new[] { _field1 });
            }

            if (!TryGetProperty(_field2, validationContext, out PropertyInfo? property2))
            {
                return new ValidationResult(string.Format(Messages.AtLeastOneRequiredUnknownProperty, _field2), new[] { _field2 });
            }

            object? value1 = property1?.GetValue(validationContext.ObjectInstance);

            object? value2 = property2?.GetValue(validationContext.ObjectInstance);

            if (!string.IsNullOrEmpty(value1 as string) || !string.IsNullOrEmpty(value2 as string))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage, new[] { _field1, _field2 });
        }

        private static bool TryGetProperty(string fieldName, ValidationContext validateionContext, out PropertyInfo? propertyInfo)
            => (propertyInfo = validateionContext.ObjectType.GetProperty(fieldName)) != null;
    }
}
