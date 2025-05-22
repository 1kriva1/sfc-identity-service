using SFC.Identity.Application.Common.Enums;
using SFC.Identity.Application.Features.Common.Base;
using SFC.Identity.Application.Features.User.Queries.Find.Dto.Filters;

namespace SFC.Identity.Application.Features.User.Queries.Find;
public class GetUsersQuery : BasePaginationRequest<GetUsersViewModel, GetUsersFilterDto>
{
    public override RequestId RequestId { get => RequestId.GetUsers; }
}