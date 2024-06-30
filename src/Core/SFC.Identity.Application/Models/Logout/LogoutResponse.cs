using AutoMapper;

using SFC.Identity.Application.Common.Extensions;
using SFC.Identity.Application.Common.Mappings;
using SFC.Identity.Application.Models.Base;
using SFC.Identity.Application.Models.Logout;

namespace SFC.Identity.Application.Models.Login;

/// <summary>
/// **Logout** response model.
/// </summary>
public class LogoutResponse : BaseErrorResponse, IMapFrom<LogoutResult>
{
    /// <summary>
    /// URL where user will be redirected after success log out.
    /// </summary>
    public string? PostLogoutRedirectUrl { get; set; }

    /// <summary>
    /// Client name.
    /// </summary>
    public string? ClientName { get; set; }

    /// <summary>
    /// URL of sign out iframe.
    /// </summary>
    public string? SignOutIFrameUrl { get; set; }

    /// <summary>
    /// Determine if user will be automatically redirected to application after success log out.
    /// </summary>
    public bool AutomaticRedirectAfterSignOut { get; set; }

    /// <summary>
    /// Determine if user must confirm logout flow.
    /// </summary>
    public bool ShowLogoutPrompt { get; set; }

    public void Mapping(Profile profile) => profile.CreateMap<LogoutResult, LogoutResponse>()
                                                   .IgnoreAllNonExisting();
}
