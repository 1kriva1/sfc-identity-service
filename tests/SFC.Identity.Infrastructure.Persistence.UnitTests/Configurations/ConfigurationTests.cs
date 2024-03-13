using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using SFC.Identity.Infrastructure.Persistence.Configurations;
using SFC.Identity.Application.Models.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using SFC.Identity.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using SFC.Identity.Domain.Entities;
using SFC.Identity.Infrastructure.Persistence.Configurations.Token;

namespace SFC.Identity.Infrastructure.Persistence.UnitTests.Configurations;
public class ConfigurationTests
{
    [Fact]
    [Trait("Persistence", "Configuration")]
    public void Persistence_Configuration_AccessToken_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        AccessTokenConfiguration sut = new();
        EntityTypeBuilder<AccessToken> builder = GetEntityBuilder<AccessToken>();

        // Act
        sut.Configure(builder);

        // Assert
        IEnumerable<IMutableProperty> properties = builder.Metadata.GetDeclaredProperties();

        Assert.Equal(4, properties.Count());

        IMutableProperty idProperty = properties.First();

        Assert.True(idProperty.IsKey());
        Assert.False(idProperty.IsColumnNullable());
        Assert.Equal(ValueGenerated.OnAdd, idProperty.ValueGenerated);
        Assert.Equal(nameof(AccessToken.Id), idProperty.Name);

        IEnumerable<IMutableForeignKey> foreignKeys = builder.Metadata.GetDeclaredReferencingForeignKeys();

        Assert.Single(foreignKeys);

        Assert.Equal(nameof(RefreshToken.Id), foreignKeys.First().Properties.First().Name);

        IEnumerable<IMutableNavigation> navigations = builder.Metadata.GetNavigations();

        Assert.Single(navigations);

        Assert.Equal(nameof(RefreshToken), navigations.First().Name);
    }

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

        IEnumerable<IMutableForeignKey> foreignKeys = builder.Metadata.GetDeclaredReferencingForeignKeys();

        Assert.Single(foreignKeys);

        Assert.Equal(nameof(AccessToken.Id), foreignKeys.First().Properties.First().Name);

        IEnumerable<IMutableNavigation> navigations = builder.Metadata.GetNavigations();

        Assert.Single(navigations);

        Assert.Equal(nameof(AccessToken), navigations.First().Name);
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

    [Fact]
    [Trait("Persistence", "Configuration")]
    public void Persistence_Configuration_RefreshToken_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        RefreshTokenConfiguration sut = new();
        EntityTypeBuilder<RefreshToken> builder = GetEntityBuilder<RefreshToken>();

        // Act
        sut.Configure(builder);

        // Assert
        IEnumerable<IMutableProperty> properties = builder.Metadata.GetDeclaredProperties();

        Assert.Equal(4, properties.Count());

        IMutableProperty idProperty = properties.First();

        Assert.True(idProperty.IsKey());
        Assert.False(idProperty.IsColumnNullable());
        Assert.Equal(ValueGenerated.OnAdd, idProperty.ValueGenerated);
        Assert.Equal(nameof(AccessToken.Id), idProperty.Name);
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
