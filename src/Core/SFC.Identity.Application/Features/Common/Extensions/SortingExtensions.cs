using System.Linq.Expressions;

using SFC.Identity.Application.Features.Common.Dto.Common;
using SFC.Identity.Application.Features.Common.Models.Find.Sorting;

namespace SFC.Identity.Application.Features.Common.Extensions;
public static class SortingExtensions
{
    public static IEnumerable<Sorting<T, dynamic>> BuildSearchSorting<T>(this IEnumerable<SortingDto> sorting,
        Func<string, Expression<Func<T, dynamic>>?> buildSortingExpression)
    {
        List<Sorting<T, dynamic>> result = [];

        sorting ??= [];

        foreach (SortingDto sort in sorting)
        {
            Expression<Func<T, dynamic>>? expression = buildSortingExpression(sort.Name);

#pragma warning disable CA1508 // Avoid dead conditional code
            if (expression is not null)
            {
                result.Add(new()
                {
                    Condition = true,
                    Direction = sort.Direction,
                    Expression = expression
                });
            }
#pragma warning restore CA1508 // Avoid dead conditional code
        }

        return result;
    }
}