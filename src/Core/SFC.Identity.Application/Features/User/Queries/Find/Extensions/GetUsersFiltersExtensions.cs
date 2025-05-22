using SFC.Identity.Application.Features.Common.Models.Find.Filters;
using SFC.Identity.Application.Features.User.Queries.Find.Dto.Filters;
using SFC.Identity.Domain.Entities.User;

namespace SFC.Identity.Application.Features.User.Queries.Find.Extensions;
public static class GetUsersFiltersExtensions
{
    public static IEnumerable<Filter<IUser>> BuildSearchFilters(this GetUsersFilterDto filter)
    {
        return [];
    }
}