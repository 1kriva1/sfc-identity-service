using System.Security.Claims;

using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

namespace SFC.Identity.Infrastructure.Services.Identity;
public class ProfileService : IProfileService
{
    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        // add actor claim for token exchange
        if (context.Subject.GetAuthenticationMethod() == OidcConstants.GrantTypes.TokenExchange)
        {
            Claim? act = context.Subject.FindFirst(JwtClaimTypes.Actor);

            if (act != null)
            {
                context.IssuedClaims.Add(act);
            }
        }

        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}
