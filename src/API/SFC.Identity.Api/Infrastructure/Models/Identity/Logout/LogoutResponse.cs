using AutoMapper;

using SFC.Identity.Api.Infrastructure.Models.Base;
using SFC.Identity.Application.Common.Extensions;
using SFC.Identity.Application.Common.Mappings.Interfaces;
using SFC.Identity.Application.Interfaces.Identity.Dto.Logout;
using SFC.Identity.Application.Interfaces.Identity.Dto.Registration;

namespace SFC.Identity.Api.Infrastructure.Models.Identity.Logout;

/// <summary>
/// **Logout** response model.
/// </summary>
public class LogoutResponse : BaseErrorResponse, IMapFrom<LogoutResultDto>
{
    /// <summary>
    /// URL where user will be redirected after success log out.
    /// </summary>
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? PostLogoutRedirectUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    /// <summary>
    /// Client name.
    /// </summary>
    public string? ClientName { get; set; }

    /// <summary>
    /// URL of sign out iframe.
    /// </summary>
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? SignOutIFrameUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    /// <summary>
    /// Determine if user will be automatically redirected to application after success log out.
    /// </summary>
    public bool AutomaticRedirectAfterSignOut { get; set; }

    /// <summary>
    /// Determine if user must confirm logout flow.
    /// </summary>
    public bool ShowLogoutPrompt { get; set; }

    public void Mapping(Profile profile) => profile.CreateMap<LogoutResultDto, LogoutResponse>()
                                                   .IgnoreAllNonExisting();
}
