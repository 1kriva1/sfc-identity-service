using System.Linq.Expressions;

using SFC.Identity.Application.Features.Common.Dto.Common;
using SFC.Identity.Application.Features.Common.Extensions;
using SFC.Identity.Application.Features.Common.Models.Find.Sorting;
using SFC.Identity.Domain.Entities.User;

namespace SFC.Identity.Application.Features.User.Queries.Find.Extensions;
public static class GetUsersSortingExtensions
{
    public static IEnumerable<Sorting<IUser, dynamic>> BuildGeneralTemplateSearchSorting(this IEnumerable<SortingDto> sorting)
        => sorting.BuildSearchSorting(BuildExpression);

    private static Expression<Func<IUser, dynamic>>? BuildExpression(string name)
    {
        return name switch
        {
            nameof(IUser.UserName) => p => p.UserName!,
            nameof(IUser.Email) => p => p.Email!,
            _ => null
        };
    }
}