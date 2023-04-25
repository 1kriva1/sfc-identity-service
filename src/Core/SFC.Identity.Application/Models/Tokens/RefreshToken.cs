namespace SFC.Identity.Application.Models.Tokens
{
    public class RefreshToken
    {
        public Guid Id { get; }

        public string Value { get; set; } = null!;

        public DateTime ExpiresDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid TokenForeignKey { get; set; }

        public AccessToken Token { get; set; } = null!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresDate;
    }
}
