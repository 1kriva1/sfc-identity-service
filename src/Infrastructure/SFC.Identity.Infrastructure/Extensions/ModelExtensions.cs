using System.Security.Claims;

using Duende.IdentityServer.Extensions;

using SFC.Identity.Application.Interfaces.Identity.Dto.Logout;

namespace SFC.Identity.Infrastructure.Extensions;

public static class ModelExtensions
{
    public static LogoutModelDto BuildLogoutModel(this ClaimsPrincipal principal, string logoutId)
    {
        return new()
        {
            LogoutId = logoutId,
            User = new LogoutUserModelDto
            {
                Id = principal.GetAuthenticationMethods().Any() ? principal.GetSubjectId() : string.Empty,
                DisplayName = principal.GetDisplayName(),
                IsAuthenticated = principal?.Identity?.IsAuthenticated ?? false
            }
        };
    }
}