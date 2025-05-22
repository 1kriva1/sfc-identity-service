namespace SFC.Identity.Infrastructure.Settings;
public class IdentitySettings
{
    public const string SectionKey = "Identity";

    public LoginSettings Login { get; set; } = default!;

    public LogoutSettings Logout { get; set; } = default!;

    public ICollection<ClientSetting> Clients { get; init; } = [];

    public ApiSettings Api { get; set; } = new ApiSettings();

    public Authentication Authentication { get; set; } = default!;
}

public class Authentication
{
    public string Authority { get; set; } = default!;

    public string Audience { get; set; } = default!;

    public IDictionary<string, IEnumerable<string>> RequireClaims { get; } = new Dictionary<string, IEnumerable<string>>();
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

    public ICollection<string> Secrets { get; init; } = [];

    public bool AllowOfflineAccess { get; set; }

    public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

    public ICollection<string> RedirectUris { get; init; } = [];

    public ICollection<string> PostLogoutRedirectUris { get; init; } = [];

    public ICollection<string> Scopes { get; init; } = [];

    public int? AccessTokenLifetime { get; set; }

    public int? IdentityTokenLifetime { get; set; }

    public int? AbsoluteRefreshTokenLifetime { get; set; }

    public int? SlidingRefreshTokenLifetime { get; set; }
}

public class ApiSettings
{
    public ICollection<ApiResourceSetting> Resources { get; init; } = [];

    public ICollection<ApiScopeSetting> Scopes { get; init; } = [];
}

public class ApiResourceSetting
{
    public string Name { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public ICollection<string> Scopes { get; init; } = [];

    public ICollection<string> UserClaims { get; init; } = [];
}

public class ApiScopeSetting
{
    public string Name { get; set; } = default!;

    public string DisplayName { get; set; } = default!;
}