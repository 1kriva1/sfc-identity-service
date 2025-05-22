using SFC.Identity.Application.Common.Mappings.Interfaces;
using SFC.Identity.Application.Interfaces.Identity.Dto.Base;

namespace SFC.Identity.Api.Infrastructure.Models.Identity.Base;
public class BaseResult : IMapFrom<BaseResultDto>
{
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string ReturnUrl { get; set; } = default!;
#pragma warning restore CA1056 // URI-like properties should not be strings

    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public object Properties { get; set; } = default!;
}