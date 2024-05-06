using SFC.Identity.Application.Common.Validators;
using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.Models.Registration;

/// <summary>
/// **Registration** model.
/// </summary>
[AtLeastOneRequired(nameof(Email), nameof(UserName))]
public class RegistrationRequest
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
}
