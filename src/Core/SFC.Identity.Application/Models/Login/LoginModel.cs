using SFC.Identity.Application.Common.Mappings;

namespace SFC.Identity.Application.Models.Login;
public class LoginModel : IMapFrom<LoginRequest>
{
    public string? Email { get; set; }

    public string? UserName { get; set; }

    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
