namespace SFC.Identity.Domain.Common.Interfaces;
public interface IAuditableEntity
{
    DateTime CreatedDate { get; set; }

    Guid CreatedBy { get; set; }

    DateTime LastModifiedDate { get; set; }

    Guid LastModifiedBy { get; set; }
}