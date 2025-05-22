namespace SFC.Identity.Messages.Models.Common;

public class Auditable
{
    public DateTime CreatedDate { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public Guid LastModifiedBy { get; set; }
}