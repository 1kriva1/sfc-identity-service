using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Domain.Entities;

namespace SFC.Identity.Infrastructure.Persistence.UnitTests;
public class IdentityDbContextTests
{
    private readonly DbContextOptions<IdentityDbContext> dbContextOptions;

    public IdentityDbContextTests()
    {
        dbContextOptions = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase($"IdentityDbContextTestsDb_{DateTime.Now.ToFileTimeUtc()}")
            .Options;
    }

    [Fact]
    [Trait("Persistence", "DbContext")]
    public void Persistence_DbContext_ShouldHasCorrectDefaultSchema()
    {
        IdentityDbContext context = new(dbContextOptions);

        string? defaultSchema = context.Model.GetDefaultSchema();

        Assert.Equal(DbConstants.DEFAULT_SCHEMA_NAME, defaultSchema);
    }

    [Fact]
    [Trait("Persistence", "DbContext")]
    public async Task Persistence_DbContext_ShouldHandleAccessTokenEntity()
    {
        // Arrange
        Guid tokenId = Guid.NewGuid();
        AccessToken entity = new()
        {
            Id= tokenId,
            CreatedDate = DateTime.UtcNow,
            ExpiresDate = DateTime.UtcNow.AddMinutes(10),
            Value = "access_token",
            RefreshToken = new RefreshToken
            {
                Id = tokenId,
                CreatedDate = DateTime.UtcNow,
                ExpiresDate = DateTime.UtcNow.AddDays(7),
                Value = "test_refresh_token"
            }
        };
        IdentityDbContext context = new(dbContextOptions);

        // Act
        EntityEntry<AccessToken> addResult = await context.AccessTokens.AddAsync(entity);
        AccessToken? accessToken = await context.AccessTokens.FindAsync(addResult.Entity.Id);

        // Assert
        Assert.NotNull(accessToken);
    }

    [Fact]
    [Trait("Persistence", "DbContext")]
    public async Task Persistence_DbContext_ShouldHandleRefreshTokenEntity()
    {
        // Arrange
        Guid tokenId = Guid.NewGuid();
        RefreshToken entity = new()
        {
            Id = tokenId,
            CreatedDate = DateTime.UtcNow,
            ExpiresDate = DateTime.UtcNow.AddDays(7),
            Value = "test_refresh_token",
            AccessToken = new AccessToken
            {
                Id = tokenId,
                CreatedDate = DateTime.UtcNow,
                ExpiresDate = DateTime.UtcNow.AddMinutes(10),
                Value = "access_token"
            }
        };
        IdentityDbContext context = new(dbContextOptions);

        // Act
        EntityEntry<RefreshToken> addResult = await context.RefreshTokens.AddAsync(entity);
        RefreshToken? refreshToken = await context.RefreshTokens.FindAsync(addResult.Entity.Id);

        // Assert
        Assert.NotNull(refreshToken);
    }
}
