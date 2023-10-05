using SFC.Identity.Application.Common.Models;

namespace SFC.Identity.Application.Models.Existence;

public class ExistenceResponse: BaseResponse
{
    public bool Exist { get; set; }
}
