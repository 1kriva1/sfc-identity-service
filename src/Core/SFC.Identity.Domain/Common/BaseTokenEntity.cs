namespace SFC.Identity.Domain.Common;
public abstract class BaseTokenEntity: BaseEntity<Guid>
{
    public string Value { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime ExpiresDate { get; set; }
}
