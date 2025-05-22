//using Duende.IdentityServer;

//using SFC.Identity.Infrastructure.Configuration;

//namespace SFC.Identity.Infrastructure.UnitTests.Configuration;
//public class IdentityConfigurationTests
//{
//    [Fact]
//    [Trait("Configuration", "Identity")]
//    public void Configuration_Identity_ShouldHaveDefinedIdentityResources()
//    {
//        // Assert
//        Assert.Equal(2, IdentityConfiguration.IdentityResources.Count());
//        Assert.Contains(IdentityConfiguration.IdentityResources, resource =>
//            resource.Name == IdentityServerConstants.StandardScopes.OpenId);
//        Assert.Contains(IdentityConfiguration.IdentityResources, resource =>
//           resource.Name == IdentityServerConstants.StandardScopes.Profile);
//    }
//}