using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.Models.Login;

/// <summary>
/// **Logout** model.
/// </summary>
public class LogoutRequest
{
    /// <summary>
    /// User identification.
    /// </summary>
    [Required(ErrorMessage = "UserIdRequired")]
    public string UserId { get; set; } = null!;
}
