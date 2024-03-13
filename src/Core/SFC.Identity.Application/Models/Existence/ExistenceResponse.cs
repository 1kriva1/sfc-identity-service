using SFC.Identity.Application.Models.Base;

namespace SFC.Identity.Application.Models.Existence;

public class ExistenceResponse: BaseResponse
{
    public bool Exist { get; set; }
}
