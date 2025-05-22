namespace SFC.Identity.Application.Interfaces.Identity.Dto.Logout;
public class LogoutResultDto
{
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? PostLogoutRedirectUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    public string? ClientName { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? SignOutIFrameUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    public bool AutomaticRedirectAfterSignOut { get; set; }

    public bool ShowLogoutPrompt { get; set; }
}