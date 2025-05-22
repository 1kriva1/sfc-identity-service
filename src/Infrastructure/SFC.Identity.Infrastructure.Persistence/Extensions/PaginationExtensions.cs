using Microsoft.EntityFrameworkCore;

using SFC.Identity.Application.Features.Common.Models.Find;
using SFC.Identity.Application.Features.Common.Models.Find.Filters;
using SFC.Identity.Application.Features.Common.Models.Find.Paging;
using SFC.Identity.Application.Features.Common.Models.Find.Sorting;
using SFC.Identity.Infrastructure.Persistence.Extensions;

namespace SFC.Identity.Infrastructure.Persistence.Extensions;
public static class PaginationExtensions
{
    #region Public

    public static async Task<PagedList<T>> PaginateAsync<T>(this IQueryable<T> query, FindParameters<T> parameters) where T : class
    {
        ArgumentNullException.ThrowIfNull(parameters);

        return await query.PaginateAsync(parameters.Pagination, parameters.Sorting, parameters.Filters)
                          .ConfigureAwait(false);
    }

    public static async Task<PagedList<T>> PaginateAsync<T>(this IQueryable<T> query, Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        int count = await query.CountAsync()
                               .ConfigureAwait(false);

        List<T> items = await query.Skip((pagination.Page - 1) * pagination.Size)
                                   .Take(pagination.Size)
                                   .ToListAsync()
                                   .ConfigureAwait(false);

        return new PagedList<T>(items, count, pagination);
    }

    public static async Task<PagedList<T>> PaginateAsync<T>(this IQueryable<T> query, Pagination pagination, Sortings<T>? sorts) where T : class
    {
        IQueryable<T> result = sorts is not null
            ? query.ApplySort(sorts)
            : query;

        return await result.PaginateAsync(pagination)
                           .ConfigureAwait(false);
    }

    public static async Task<PagedList<T>> PaginateAsync<T>(this IQueryable<T> query, Pagination pagination, Sortings<T>? sorts, Filters<T>? filters) where T : class
    {
        IQueryable<T> results = query.ApplyFilter(filters);

        return await results.PaginateAsync(pagination, sorts)
                            .ConfigureAwait(false);
    }

    #endregion Public

    #region Private

    private static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Filters<T>? filters)
    {
        IQueryable<T> results = filters is not null && filters.IsValid
            ? filters.FilterList.Aggregate(query, (current, filter) => current.Where(filter.Expression))
            : query;

        return results;
    }

    private static IQueryable<T> ApplySort<T>(this IQueryable<T> query, Sortings<T> sorts) where T : class
    {
        if (!sorts.IsValid) return query.AsSingleQuery();

        IEnumerable<Sorting<T, dynamic>> sorting = sorts.Get();

        return sorting != null ? sorts.ApplySort(query, sorting) : query;
    }

    #endregion Private
}