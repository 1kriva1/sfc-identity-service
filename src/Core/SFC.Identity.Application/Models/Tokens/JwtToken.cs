namespace SFC.Identity.Application.Models.Tokens;

/// <summary>
/// **JWT** model.
/// </summary>
public class JwtToken
{
    /// <summary>
    /// Access token value.
    /// </summary>
    public string Access { get; set; } = null!;

    /// <summary>
    /// Refresh token value.
    /// </summary>
    public string Refresh { get; set; } = null!;
}
