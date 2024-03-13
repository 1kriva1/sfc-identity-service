using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Domain.Entities;
using SFC.Identity.Infrastructure.Services;
using SFC.Identity.Infrastructure.Settings;

using System.Security.Claims;
using Xunit;

namespace SFC.Identity.Infrastructure.UnitTests.Services;

public class JwtServiceTests
{
    private readonly JwtService _service;

    private readonly JwtSettings _settings = new()
    {
        Key = "key_ahsvdjavsdvqwyvetyqweyvasndvhgavsdghcvahsdc",
        Issuer = "test_issuer",
        Audience = "test_audience",
        RefreshTokenDurationInDays = 7,
        AccessTokenDurationInMinutes = 2
    };

    public JwtServiceTests()
    {
        Mock<IOptions<JwtSettings>> jwtSettingsOptionsMock = new();

        jwtSettingsOptionsMock.Setup(s => s.Value).Returns(_settings);

        _service = new(jwtSettingsOptionsMock.Object);
    }

    [Fact]
    [Trait("Service", "Jwt")]
    public void Service_Jwt_GenerateAccessToken_ShouldGenerateToken()
    {
        // Arrange
        IEnumerable<Claim> authClaims = new List<Claim> { };

        // Act
        AccessToken token = _service.GenerateAccessToken(authClaims);

        // Assert
        Assert.NotNull(token);
        Assert.False(string.IsNullOrEmpty(token.Value));
        Assert.Equal(_settings.AccessTokenDurationInMinutes, Math.Ceiling((token.ExpiresDate - token.CreatedDate).TotalMinutes));
        Assert.Null(token.RefreshToken);
    }

    [Fact]
    [Trait("Service", "Jwt")]
    public void Service_Jwt_GenerateRefreshToken_ShouldGenerateToken()
    {
        // Act
        RefreshToken token = _service.GenerateRefreshToken();

        // Assert
        Assert.NotNull(token);
        Assert.False(string.IsNullOrEmpty(token.Value));
        Assert.Equal(_settings.RefreshTokenDurationInDays, Math.Floor((token.ExpiresDate - token.CreatedDate).TotalDays));
    }

    [Fact]
    [Trait("Service", "Jwt")]
    public void Service_Jwt_GetPrincipalFromExpiredToken_ShouldReturnPrincipal()
    {
        // Arrange
        IEnumerable<Claim> authClaims = new List<Claim> { new Claim("test_type", "test_value") };

        AccessToken token = _service.GenerateAccessToken(authClaims);

        // Act
        ClaimsPrincipal? principal = _service.GetPrincipalFromExpiredToken(token.Value);

        // Assert
        Assert.NotNull(principal);
        Assert.NotEmpty(principal.Identities);
    }

    [Fact]
    [Trait("Service", "Jwt")]
    public void Service_Jwt_GetPrincipalFromExpiredToken_ShouldHaveDefinedClaims()
    {
        // Arrange
        string claimType = "test_type", claimValue = "test_value";

        IEnumerable<Claim> authClaims = new List<Claim> { new Claim(claimType, claimValue) };

        AccessToken token = _service.GenerateAccessToken(authClaims);

        // Act
        ClaimsPrincipal? principal = _service.GetPrincipalFromExpiredToken(token.Value);

        // Assert
        Assert.NotNull(principal);
        Assert.Contains(principal.Claims, claim => claim.Type == claimType);
        Assert.Equal(claimValue, principal.Claims.First(v => v.Type == claimType).Value);
        Assert.Equal(_settings.Issuer, principal.Claims.First(v => v.Type == "iss").Value);
        Assert.Equal(_settings.Audience, principal.Claims.First(v => v.Type == "aud").Value);
    }

    [Fact]
    [Trait("Service", "Jwt")]
    public void Service_Jwt_GetPrincipalFromExpiredToken_ShouldThrowSecurityTokenSignatureKeyNotFoundExceptionWhenTokenInvalid()
    {
        // Arrange
        string invalidAccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2ODIxOTA1MDksImlzcyI6InRlc3RfaXNzdWVyIiwiYXVkIjoidGVzdF9hdWRpZW5jZSJ9.xXSkSm9tTJZrdA_ucX_J6CrBIfmMZ-I2LFTKsao7tI8";

        // Assert
        Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() => _service.GetPrincipalFromExpiredToken(invalidAccessToken));
    }
}
