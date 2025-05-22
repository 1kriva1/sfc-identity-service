using System.ComponentModel.DataAnnotations;

using SFC.Identity.Api.Infrastructure.Validators;
using SFC.Identity.Application.Common.Mappings.Interfaces;
using SFC.Identity.Application.Interfaces.Identity.Dto.Login;

namespace SFC.Identity.Api.Infrastructure.Models.Identity.Login;

/// <summary>
/// **Login** model.
/// </summary>
[AtLeastOneRequired(nameof(Email), nameof(UserName))]
public class LoginRequest : IMapTo<LoginModelDto>
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

    /// <summary>
    /// Define if login persistent.
    /// </summary>
    public bool RememberMe { get; set; }

    /// <summary>
    /// Return URL.
    /// </summary>
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? ReturnUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings
}