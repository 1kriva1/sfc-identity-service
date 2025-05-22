namespace SFC.Identity.Api.Infrastructure.Models.Pagination;

/// <summary>
/// **Pagination** links model.
/// </summary>
public class PageLinksModel
{
    /// <summary>
    /// Link to first page.
    /// </summary>
    public Uri FirstPage { get; set; } = default!;

    /// <summary>
    /// Link to last page.
    /// </summary>
    public Uri LastPage { get; set; } = default!;

    /// <summary>
    /// Link to next page.
    /// </summary>
    public Uri? NextPage { get; set; }

    /// <summary>
    /// Link to previous page.
    /// </summary>
    public Uri? PreviousPage { get; set; }
}