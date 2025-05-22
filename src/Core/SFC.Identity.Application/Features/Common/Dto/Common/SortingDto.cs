using SFC.Identity.Application.Common.Enums;

namespace SFC.Identity.Application.Features.Common.Dto.Common;

/// <summary>
/// Sorting DTO.
/// </summary>
public class SortingDto
{
    public required string Name { get; set; }

    public SortingDirection Direction { get; set; }
}