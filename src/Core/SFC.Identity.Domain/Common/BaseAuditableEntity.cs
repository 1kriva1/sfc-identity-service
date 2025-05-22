using SFC.Identity.Domain.Common.Interfaces;

namespace SFC.Identity.Domain.Common;

public abstract class BaseAuditableEntity<TEntity> : BaseEntity<TEntity>, IAuditableEntity
{
    public DateTime CreatedDate { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public Guid LastModifiedBy { get; set; }
}