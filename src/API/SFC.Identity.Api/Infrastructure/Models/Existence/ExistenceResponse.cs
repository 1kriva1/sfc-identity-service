using SFC.Identity.Api.Infrastructure.Models.Base;

namespace SFC.Identity.Api.Infrastructure.Models.Existence;

/// <summary>
/// Described the result of check user on **existence**.
/// </summary>
public class ExistenceResponse : BaseResponse
{
    /// <summary>
    /// Determined if User exist or not.
    /// </summary>
    public bool Exist { get; set; }
}
