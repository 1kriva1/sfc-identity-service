using System.Security.Claims;

using IdentityModel;

using SFC.Identity.Api.Extensions;
using SFC.Identity.Application.Models.Logout;

using Xunit;

namespace SFC.Identity.Api.UnitTests.Extensions;
public class ModelExtensionsTests
{
    [Fact]
    [Trait("Extension", "Model")]
    public void Extension_Model_ShouldBuildLogoutModel()
    {
        // Arrange
        string logoutId = "logout_id", subjectValue = "subject";
        ClaimsPrincipal principal = new(new ClaimsIdentity(new List<Claim> {
                new(JwtClaimTypes.AuthenticationMethod, OidcConstants.GrantTypes.AuthorizationCode),
                new(JwtClaimTypes.Subject, subjectValue)
        }));

        // Act
        LogoutModel result = principal.BuildLogoutModel(logoutId);

        // Assert
        Assert.Equal(logoutId, result.LogoutId);
        Assert.Equal(subjectValue, result.User.Id);
        Assert.Equal(subjectValue, result.User.DisplayName);
        Assert.False(result.User.IsAuthenticated);
    }

    [Fact]
    [Trait("Extension", "Model")]
    public void Extension_Model_ShouldBuildLogoutModelWithEmptySubject()
    {
        // Arrange
        string logoutId = "logout_id", subjectValue = "subject";
        ClaimsPrincipal principal = new(new ClaimsIdentity(new List<Claim> {
                new(JwtClaimTypes.Subject, subjectValue)
        }));

        // Act
        LogoutModel result = principal.BuildLogoutModel(logoutId);

        // Assert
        Assert.Equal(logoutId, result.LogoutId);
        Assert.Empty(result.User.Id);
        Assert.Equal(subjectValue, result.User.DisplayName);
        Assert.False(result.User.IsAuthenticated);
    }
}
