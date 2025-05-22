using System.Globalization;
using System.Text;

using FluentValidation;

namespace SFC.Identity.Application.Common.Extensions;
public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> RequiredProperty<T>(this IRuleBuilderInitial<T, string> builder,
        int? maximumLength = null, string? propertyName = null)
    {
        IRuleBuilderOptions<T, string> options = builder.NotEmpty();

        if (maximumLength.HasValue)
        {
            options.MaximumLength(maximumLength.Value);
        }

        if (!string.IsNullOrEmpty(propertyName))
        {
            options.WithName(propertyName);
        }

        return options;
    }

    public static string BuildValidationMessage(this string value, params object[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, CompositeFormat.Parse(value), args);
    }

    public static IRuleBuilderOptions<T, TProperty> WithException<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Exception exception)
    {
        return rule.WithState(x => exception);
    }
}