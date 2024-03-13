using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Domain.Entities;
using SFC.Identity.Infrastructure.Settings;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SFC.Identity.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public AccessToken GenerateAccessToken(IEnumerable<Claim> authClaims)
    {
        SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        DateTime expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenDurationInMinutes);

        JwtSecurityToken jwtSecurityToken = new(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: authClaims,
            expires: expires,
            signingCredentials: signingCredentials);

        return new AccessToken
        {
            CreatedDate = DateTime.UtcNow,
            ExpiresDate = expires,
            Value = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
        };
    }

    public RefreshToken GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[64];

        using RandomNumberGenerator rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        return new RefreshToken
        {
            CreatedDate = DateTime.UtcNow,
            ExpiresDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays),
            Value = Convert.ToBase64String(randomNumber)
        };
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken)
    {
        TokenValidationParameters parameters = new()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateLifetime = false
        };

        ClaimsPrincipal principal = new JwtSecurityTokenHandler()
            .ValidateToken(accessToken, parameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException(Messages.ValidationError,
                ("AccessToken", Messages.TokenInvalid));

        return principal;
    }
}
