using SFC.Identity.Application.Models.Base;
using SFC.Identity.Application.Models.Tokens;

namespace SFC.Identity.Application.Models.RefreshToken;

/// <summary>
/// **Refresh token** response model.
/// </summary>
public class RefreshTokenResponse: BaseResponse
{
    /// <summary>
    /// Refreshed JWT token.
    /// </summary>
    public JwtToken Token { get; set; } = null!;
}
