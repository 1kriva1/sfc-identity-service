using SFC.Identity.Application.Common.Mappings.Interfaces;
using SFC.Identity.Application.Features.Common.Dto.Pagination;

namespace SFC.Identity.Api.Infrastructure.Models.Pagination;

/// <summary>
/// **Pagination** metadata model.
/// </summary>
public class PageMetadataModel : IMapFrom<PageMetadataDto>
{
    /// <summary>
    /// **Current** page.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total **pages** count.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Page **size**.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total **items** count.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Describe if **previous** page **exist** for defined filters and page size.
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Describe if **next** page **exist** for defined filters and page size.
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Links model.
    /// </summary>
    public PageLinksModel Links { get; set; } = default!;
}