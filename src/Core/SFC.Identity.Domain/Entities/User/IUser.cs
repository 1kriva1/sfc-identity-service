using SFC.Identity.Domain.Common.Interfaces;

namespace SFC.Identity.Domain.Entities.User;
public interface IUser : IAuditableUserEntity
{
    public string? UserName { get; set; }

    public string? Email { get; set; }
}