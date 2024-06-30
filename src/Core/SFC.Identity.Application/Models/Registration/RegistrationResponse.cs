using SFC.Identity.Application.Common.Mappings;
using SFC.Identity.Application.Models.Base;

namespace SFC.Identity.Application.Models.Registration;

/// <summary>
/// **Registration** response model.
/// </summary>
public class RegistrationResponse : BaseResponse, IMapFrom<RegistrationResult>
{
    /// <summary>
    /// Return URL to application.
    /// </summary>
    public string ReturnUrl { get; set; } = default!;
}
