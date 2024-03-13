using SFC.Identity.Application.Models.Base;
using SFC.Identity.Application.Models.Tokens;

namespace SFC.Identity.Application.Models.RefreshToken;

public class RefreshTokenResponse: BaseResponse
{
    public JwtToken Token { get; set; } = null!;
}
