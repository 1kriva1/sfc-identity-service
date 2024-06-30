using Duende.IdentityServer.Models;

using SFC.Identity.Application.Common.Constants;

namespace SFC.Identity.Infrastructure.Configuration;
public static class IdentityConfiguration
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];
}
