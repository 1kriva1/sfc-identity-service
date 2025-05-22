using SFC.Identity.Application.Features.Common.Dto.Pagination;

namespace SFC.GeneralTemplate.Application.Features.Common.Dto.Pagination;

/// <summary>
/// Parent class for find(page) results.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PageDto<T>
{
    /// <summary>
    /// Items of the page.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = default!;

    /// <summary>
    /// Page metadata.
    /// </summary>
    public PageMetadataDto Metadata { get; set; } = default!;
}