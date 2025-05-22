using Microsoft.AspNetCore.Identity;

using SFC.Identity.Application.Common.Dto.User;
using SFC.Identity.Application.Common.Mappings.Interfaces;
using SFC.Identity.Domain.Common.Interfaces;
using SFC.Identity.Domain.Entities.User;

namespace SFC.Identity.Infrastructure.Persistence.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAuditableUserEntity, IUser, IMapTo<UserDto>
{
    public DateTime CreatedDate { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public Guid LastModifiedBy { get; set; }
}