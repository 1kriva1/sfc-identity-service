namespace SFC.Identity.Infrastructure.Settings;
public class IdentitySettings
{
    public const string SECTION_KEY = "Identity";

    public LoginSettings Login { get; set; } = default!;

    public LogoutSettings Logout { get; set; } = default!;

    public ICollection<ClientSetting> Clients { get; set; } = [];

    public ApiSettings Api { get; set; } = new ApiSettings();
}

public class LoginSettings
{
    // Determine if user can set remember me during login
    public bool AllowRememberLogin { get; set; }

    /// <summary>
    /// Remember me duration in seconds
    /// </summary>
    public int RememberLoginDuration { get; set; }
}

public class LogoutSettings
{
    /// <summary>
    /// Show prompt question before logout
    /// </summary>
    public bool ShowLogoutPrompt { get; set; }

    /// <summary>
    /// Determine if user will be automaticlly redirected to application after logout
    /// </summary>
    public bool AutomaticRedirectAfterSignOut { get; set; }
}

public class ClientSetting
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public bool IsTokenExchange { get; set; }

    public ICollection<string> Secrets { get; set; } = [];

    public bool AllowOfflineAccess { get; set; }

    public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

    public ICollection<string> RedirectUris { get; set; } = [];

    public ICollection<string> PostLogoutRedirectUris { get; set; } = [];

    public ICollection<string> Scopes { get; set; } = [];

    public int? AccessTokenLifetime { get; set; }

    public int? IdentityTokenLifetime { get; set; }

    public int? AbsoluteRefreshTokenLifetime { get; set; }

    public int? SlidingRefreshTokenLifetime { get; set; }
}

public class ApiSettings
{
    public ICollection<ApiResourceSetting> Resources { get; set; } = [];

    public ICollection<ApiScopeSetting> Scopes { get; set; } = [];
}

public class ApiResourceSetting
{
    public string Name { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public ICollection<string> Scopes { get; set; } = [];

    public ICollection<string> UserClaims { get; set; } = [];
}

public class ApiScopeSetting
{
    public string Name { get; set; } = default!;

    public string DisplayName { get; set; } = default!;
}