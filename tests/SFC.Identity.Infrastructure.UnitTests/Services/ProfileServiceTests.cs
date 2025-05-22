//using System.Security.Claims;

//using Duende.IdentityModel;
//using Duende.IdentityServer.Models;

//using SFC.Identity.Infrastructure.Services;

//namespace SFC.Identity.Infrastructure.UnitTests.Services;
//public class ProfileServiceTests
//{
//    [Fact]
//    [Trait("Service", "Profile")]
//    public async Task Service_Profile_ShouldReturnIsActive()
//    {
//        // Arrange
//        ClaimsPrincipal principal = new();
//        Client client = new();
//        IsActiveContext context = new(principal, client, nameof(ProfileServiceTests));
//        ProfileService service = new();

//        // Act
//        await service.IsActiveAsync(context);

//        // Assert
//        Assert.True(context.IsActive);
//    }

//    [Fact]
//    [Trait("Service", "Profile")]
//    public async Task Service_Profile_ShouldNoContainActorClaimWhenAuthenticationMethodIsNotExchange()
//    {
//        // Arrange
//        ProfileDataRequestContext context = new()
//        {
//            Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
//                new(JwtClaimTypes.AuthenticationMethod, OidcConstants.GrantTypes.AuthorizationCode)
//            }))
//        };
//        ProfileService service = new();

//        // Act
//        await service.GetProfileDataAsync(context);

//        // Assert
//        Assert.DoesNotContain(context.IssuedClaims, claim => claim.Type == JwtClaimTypes.Actor);
//    }

//    [Fact]
//    [Trait("Service", "Profile")]
//    public async Task Service_Profile_ShouldNoContainActorClaimWhenContextDoesNotContainActorClaim()
//    {
//        // Arrange
//        ProfileDataRequestContext context = new()
//        {
//            Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
//                new(JwtClaimTypes.AuthenticationMethod, OidcConstants.GrantTypes.TokenExchange)
//            }))
//        };
//        ProfileService service = new();

//        // Act
//        await service.GetProfileDataAsync(context);

//        // Assert
//        Assert.DoesNotContain(context.IssuedClaims, claim => claim.Type == JwtClaimTypes.Actor);
//    }

//    [Fact]
//    [Trait("Service", "Profile")]
//    public async Task Service_Profile_ShouldContainActorClaim()
//    {
//        // Arrange
//        string assertActValue = "act";
//        ProfileDataRequestContext context = new()
//        {
//            Subject = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
//                new(JwtClaimTypes.AuthenticationMethod, OidcConstants.GrantTypes.TokenExchange),
//                new(JwtClaimTypes.Actor, assertActValue)
//            }))
//        };
//        ProfileService service = new();

//        // Act
//        await service.GetProfileDataAsync(context);

//        // Assert
//        Assert.Contains(context.IssuedClaims, claim => claim.Type == JwtClaimTypes.Actor);
//        Assert.Equal(assertActValue, context.IssuedClaims.First(c => c.Type == JwtClaimTypes.Actor).Value);
//    }
//}