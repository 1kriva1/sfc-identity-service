namespace SFC.Identity.Domain.Common.Interfaces;
public interface IAuditableUserEntity : IAuditableEntity
{
    Guid Id { get; set; }
}