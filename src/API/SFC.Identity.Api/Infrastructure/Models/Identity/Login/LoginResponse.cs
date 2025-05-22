using SFC.Identity.Api.Infrastructure.Models.Base;
using SFC.Identity.Application.Common.Mappings.Interfaces;
using SFC.Identity.Application.Interfaces.Identity.Dto.Login;

namespace SFC.Identity.Api.Infrastructure.Models.Identity.Login;

/// <summary>
/// **Login** response model.
/// </summary>
public class LoginResponse : BaseResponse, IMapFrom<LoginResultDto>
{
    /// <summary>
    /// Return URL to application.
    /// </summary>
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string ReturnUrl { get; set; } = default!;
#pragma warning restore CA1056 // URI-like properties should not be strings
}