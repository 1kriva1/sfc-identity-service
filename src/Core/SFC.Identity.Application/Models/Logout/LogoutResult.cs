namespace SFC.Identity.Application.Models.Logout;
public class LogoutResult
{
    public string? PostLogoutRedirectUrl { get; set; }

    public string? ClientName { get; set; }

    public string? SignOutIFrameUrl { get; set; }

    public bool AutomaticRedirectAfterSignOut { get; set; }

    public bool ShowLogoutPrompt { get; set; }
}
