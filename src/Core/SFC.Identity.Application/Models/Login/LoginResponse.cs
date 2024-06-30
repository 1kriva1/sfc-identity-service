using SFC.Identity.Application.Common.Mappings;
using SFC.Identity.Application.Models.Base;

namespace SFC.Identity.Application.Models.Login;

/// <summary>
/// **Login** response model.
/// </summary>
public class LoginResponse: BaseResponse, IMapFrom<LoginResult>
{
    /// <summary>
    /// Return URL to application.
    /// </summary>
    public string ReturnUrl { get; set; } = default!;
}
