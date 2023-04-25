using SFC.Identity.Application.Models.Tokens;
using System.Security.Claims;

namespace SFC.Identity.Application.Interfaces
{
    public interface IJwtService
    {
        AccessToken GenerateAccessToken(IEnumerable<Claim> userClaims);

        RefreshToken GenerateRefreshToken();

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);
    }
}
