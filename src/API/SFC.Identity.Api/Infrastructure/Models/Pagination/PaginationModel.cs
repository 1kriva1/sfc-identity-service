using SFC.Identity.Application.Common.Mappings.Interfaces;
using SFC.Identity.Application.Features.Common.Dto.Pagination;

namespace SFC.Identity.Api.Infrastructure.Models.Pagination;

/// <summary>
/// **Pagination** model.
/// </summary>
public class PaginationModel : IMapTo<PaginationDto>
{
    /// <summary>
    /// Requested **page**.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Requested page **size**.
    /// </summary>
    public int Size { get; set; }
}