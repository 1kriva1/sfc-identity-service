using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Tokens;
using System.Security.Claims;

namespace SFC.Identity.Infrastructure.Extensions
{
    public static class TokenExtensions
    {
        public static AccessToken CreateAccessToken(this IJwtService jwtService, IEnumerable<Claim> userClaims)
        {

            AccessToken token = jwtService.GenerateToken(userClaims);

            RefreshToken refreshToken = jwtService.GenerateRefreshToken();

            token.RefreshToken = refreshToken;

            return token;
        }
    }
}
