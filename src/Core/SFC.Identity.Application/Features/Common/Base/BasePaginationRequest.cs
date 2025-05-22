using SFC.Identity.Application.Features.Common.Dto.Common;
using SFC.Identity.Application.Features.Common.Dto.Pagination;

namespace SFC.Identity.Application.Features.Common.Base;

/// <summary>
/// Base class for find query.
/// </summary>
/// <typeparam name="TResponse">Response type.</typeparam>
/// <typeparam name="TFilter">Filter type.</typeparam>
public abstract class BasePaginationRequest<TResponse, TFilter> : Request<TResponse>
{
    /// <summary>
    /// Pagination.
    /// </summary>
    public PaginationDto Pagination { get; set; } = default!;

    /// <summary>
    /// Sorting list.
    /// </summary>
    public IEnumerable<SortingDto> Sorting { get; set; } = [];

    /// <summary>
    /// Filter.
    /// </summary>
    public TFilter Filter { get; set; } = default!;
}