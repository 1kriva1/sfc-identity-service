using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using SFC.Identity.Infrastructure.Persistence.Configurations.Identity;
using SFC.Identity.Infrastructure.Persistence.Entities;

namespace SFC.Identity.Infrastructure.Persistence.UnitTests.Configurations;
public class ConfigurationTests
{
    [Fact]
    [Trait("Persistence", "Configuration")]
    public void Persistence_Configuration_ApplicationRole_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        ApplicationRoleConfiguration sut = new();
        EntityTypeBuilder<ApplicationRole> builder = GetEntityBuilder<ApplicationRole>();

        // Act
        sut.Configure(builder);

        // Assert
        Assert.Equal("Roles", builder.Metadata.GetTableName());
    }

    [Fact]
    [Trait("Persistence", "Configuration")]
    public void Persistence_Configuration_ApplicationUser_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        ApplicationUserConfiguration sut = new();
        EntityTypeBuilder<ApplicationUser> builder = GetEntityBuilder<ApplicationUser>();

        // Act
        sut.Configure(builder);

        // Assert
        Assert.Equal("Users", builder.Metadata.GetTableName());
    }

    [Fact]
    [Trait("Persistence", "Configuration")]
    public void Persistence_Configuration_IdentityRoleClaim_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        IdentityRoleClaimConfiguration sut = new();
        EntityTypeBuilder<IdentityRoleClaim<Guid>> builder = GetEntityBuilder<IdentityRoleClaim<Guid>>();

        // Act
        sut.Configure(builder);

        // Assert
        Assert.Equal("RoleClaims", builder.Metadata.GetTableName());
    }

    [Fact]
    [Trait("Persistence", "Configuration")]
    public void Persistence_Configuration_IdentityUserClaim_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        IdentityUserClaimConfiguration sut = new();
        EntityTypeBuilder<IdentityUserClaim<Guid>> builder = GetEntityBuilder<IdentityUserClaim<Guid>>();

        // Act
        sut.Configure(builder);

        // Assert
        Assert.Equal("UserClaims", builder.Metadata.GetTableName());
    }

    [Fact]
    [Trait("Persistence", "Configuration")]
    public void Persistence_Configuration_IdentityUserLogin_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        IdentityUserLoginConfiguration sut = new();
        EntityTypeBuilder<IdentityUserLogin<Guid>> builder = GetEntityBuilder<IdentityUserLogin<Guid>>();

        // Act
        sut.Configure(builder);

        // Assert
        Assert.Equal("UserLogins", builder.Metadata.GetTableName());
    }

    [Fact]
    [Trait("Persistence", "Configuration")]
    public void Persistence_Configuration_IdentityUserRole_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        IdentityUserRoleConfiguration sut = new();
        EntityTypeBuilder<IdentityUserRole<Guid>> builder = GetEntityBuilder<IdentityUserRole<Guid>>();

        // Act
        sut.Configure(builder);

        // Assert
        Assert.Equal("UserRoles", builder.Metadata.GetTableName());
    }

    [Fact]
    [Trait("Persistence", "Configuration")]
    public void Persistence_Configuration_IdentityUserToken_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        IdentityUserTokenConfiguration sut = new();
        EntityTypeBuilder<IdentityUserToken<Guid>> builder = GetEntityBuilder<IdentityUserToken<Guid>>();

        // Act
        sut.Configure(builder);

        // Assert
        Assert.Equal("UserTokens", builder.Metadata.GetTableName());
    }

    private static EntityTypeBuilder<T> GetEntityBuilder<T>() where T : class
    {
#pragma warning disable EF1001 // Internal EF Core API usage.
        EntityType entityType = new(typeof(T).Name, typeof(T), new Model(), false, ConfigurationSource.Explicit);

        EntityTypeBuilder<T> builder = new(entityType);

        return builder;
#pragma warning restore EF1001 // Internal EF Core API usage.
    }
}