namespace SFC.Identity.Application.Models.Tokens;

public class JwtToken
{
    public string Access { get; set; } = null!;

    public string Refresh { get; set; } = null!;
}
