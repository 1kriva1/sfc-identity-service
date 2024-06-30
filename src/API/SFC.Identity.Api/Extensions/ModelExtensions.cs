using Duende.IdentityServer.Extensions;

using SFC.Identity.Application.Models.Logout;

using System.Security.Claims;

namespace SFC.Identity.Api.Extensions;

public static class ModelExtensions
{
    public static LogoutModel BuildLogoutModel(this ClaimsPrincipal principal, string logoutId)
    {
        return new()
        {
            LogoutId = logoutId,
            User = new LogoutUserModel
            {
                Id = principal.GetAuthenticationMethods().Any() ? principal.GetSubjectId() : string.Empty,
                DisplayName = principal.GetDisplayName(),
                IsAuthenticated = principal?.Identity?.IsAuthenticated ?? false
            }
        };
    }
}
