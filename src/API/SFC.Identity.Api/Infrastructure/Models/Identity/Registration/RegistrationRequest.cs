using SFC.Identity.Api.Infrastructure.Validators;
using SFC.Identity.Application.Common.Mappings.Interfaces;
using SFC.Identity.Application.Interfaces.Identity.Dto.Registration;

using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Api.Infrastructure.Models.Identity.Registration;

/// <summary>
/// **Registration** model.
/// </summary>
[AtLeastOneRequired(nameof(Email), nameof(UserName))]
public class RegistrationRequest : IMapTo<RegistrationModelDto>
{
    /// <summary>
    /// Email address (required if UserName is missed).
    /// </summary>
    [EmailAddress(ErrorMessage = "EmailInvalid")]
    public string? Email { get; set; }

    /// <summary>
    /// User name (required if Email is missed).
    /// </summary>
    [RegularExpression("^[a-zA-Z0-9-._@+]+$", ErrorMessage = "UserNameInvalid")]
    public string? UserName { get; set; }

    /// <summary>
    /// Password value.
    /// </summary>
    [Required(ErrorMessage = "PasswordRequired")]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,}$",
        ErrorMessage = "PasswordInvalid")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Confirm password value (must be equal to Password value).
    /// </summary>
    [Required(ErrorMessage = "ConfirmPasswordRequired")]
    [Compare(nameof(Password), ErrorMessage = "ConfirmPasswordInvalid")]
    public string ConfirmPassword { get; set; } = null!;

    /// <summary>
    /// Return URI.
    /// </summary>
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? ReturnUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings
}
