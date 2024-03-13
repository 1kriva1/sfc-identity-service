namespace SFC.Identity.Infrastructure.Settings;
public class JwtSettings
{
    public string Key { get; set; } = null!;

    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;

    public double AccessTokenDurationInMinutes { get; set; }

    public int RefreshTokenDurationInDays { get; set; }
}
