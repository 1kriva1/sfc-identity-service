using Microsoft.Extensions.Options;
using Moq;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Infrastructure.Services;
using System.Security.Claims;
using SFC.Identity.Infrastructure.Extensions;
using Xunit;
using SFC.Identity.Domain.Entities;
using SFC.Identity.Infrastructure.Settings;

namespace SFC.Identity.Infrastructure.UnitTests.Extensions;

public class TokenExtensionsTests
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

    public TokenExtensionsTests()
    {
        Mock<IOptions<JwtSettings>> jwtSettingsOptionsMock = new();

        jwtSettingsOptionsMock.Setup(s => s.Value).Returns(_settings);

        _service = new(jwtSettingsOptionsMock.Object);
    }

    [Fact]
    [Trait("Extension", "Jwt")]
    public void Extension_Jwt_ShouldCreateAccessToken()
    {
        // Arrange
        IEnumerable<Claim> authClaims = new List<Claim> { };

        // Act
        AccessToken token = _service.CreateAccessToken(authClaims);

        // Assert
        Assert.NotNull(token);
        Assert.NotNull(token.RefreshToken);
    }
}
