using Duende.IdentityServer.Models;

namespace SFC.Identity.Infrastructure.Configuration;
public static class IdentityConfiguration
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        // subject id, user id will be returned and etc.
        new IdentityResources.OpenId(),
        // profile related data returned (given name, family name...)
        new IdentityResources.Profile()
    ];
}