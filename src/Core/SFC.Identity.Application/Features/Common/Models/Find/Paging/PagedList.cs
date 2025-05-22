namespace SFC.Identity.Application.Features.Common.Models.Find.Paging;

/// <summary>
/// List of items and metadata information.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public class PagedList<TEntity> : List<TEntity>
{
    public int CurrentPage { get; private set; }

    public int TotalPages { get; private set; }

    public int PageSize { get; private set; }

    public int TotalCount { get; private set; }

    public PagedList(IEnumerable<TEntity> items, int count, Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        TotalCount = count;
        PageSize = pagination.Size;
        CurrentPage = pagination.Page;
        TotalPages = (int)Math.Ceiling(count / (double)pagination.Size);
        AddRange(items);
    }
}