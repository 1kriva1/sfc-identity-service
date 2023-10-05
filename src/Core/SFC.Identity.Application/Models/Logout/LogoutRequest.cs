using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.Models.Login;

public class LogoutRequest
{
    [Required(ErrorMessage = "UserIdRequired")]
    public string UserId { get; set; } = null!;
}
