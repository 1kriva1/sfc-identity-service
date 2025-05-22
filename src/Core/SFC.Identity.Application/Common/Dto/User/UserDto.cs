using SFC.Identity.Application.Common.Dto.Common;
using SFC.Identity.Application.Common.Mappings.Interfaces;
using SFC.Identity.Domain.Entities.User;

namespace SFC.Identity.Application.Common.Dto.User;
public class UserDto : AuditableDto, IMapFrom<IUser>
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }
}