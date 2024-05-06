using SFC.Identity.Application.Common.Validators;
using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.Models.Login;

/// <summary>
/// **Login** model.
/// </summary>
[AtLeastOneRequired(nameof(Email), nameof(UserName))]
public class LoginRequest
{
    /// <summary>
    /// User email.
    /// </summary>
    [EmailAddress(ErrorMessage = "EmailInvalid")]
    public string? Email { get; set; }

    /// <summary>
    /// User name.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    [Required(ErrorMessage = "PasswordRequired")]
    public string Password { get; set; } = null!;
}
