namespace SFC.Identity.Application.Models.Tokens;

public class AccessToken
{
    public Guid Id { get; }

    public string Value { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime ExpiresDate { get; set; }

    public Guid UserForeignKey { get; set; }

    public RefreshToken RefreshToken { get; set; } = null!;        
}
