using SFC.Identity.Application.Models.Base;

namespace SFC.Identity.Application.Models.Existence;

/// <summary>
/// Described the result of check user on **existence**.
/// </summary>
public class ExistenceResponse: BaseResponse
{
    /// <summary>
    /// Determined if User exist or not.
    /// </summary>
    public bool Exist { get; set; }
}
