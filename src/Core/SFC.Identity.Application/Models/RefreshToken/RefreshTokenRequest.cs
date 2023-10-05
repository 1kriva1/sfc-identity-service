using SFC.Identity.Application.Models.Tokens;
using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.Models.RefreshToken;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "TokenRequired")]
    public JwtToken Token { get; set; } = null!;
}
