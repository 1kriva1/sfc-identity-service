using SFC.Identity.Application.Models.Base;
using SFC.Identity.Application.Models.Tokens;

namespace SFC.Identity.Application.Models.Login;

/// <summary>
/// **Login** response model.
/// </summary>
public class LoginResponse: BaseResponse
{
    /// <summary>
    /// User identification.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// JWT token.
    /// </summary>
    public JwtToken Token { get; set; } = null!;
}
