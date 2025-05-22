using Microsoft.AspNetCore.Authorization;

using SFC.Identity.Infrastructure.Authorization;

namespace SFC.Identity.Infrastructure.Extensions;
public static class AuthorizationExtensions
{
    public static void AddGeneralPolicy(this AuthorizationOptions options, IDictionary<string, IEnumerable<string>> claims)
    {
        PolicyModel general = AuthorizationPolicies.General(claims);
        options.AddPolicy(general.Name, general.Policy);
    }
}