using SFC.Identity.Application.Common.Mappings;
using SFC.Identity.Application.Models.Login;

namespace SFC.Identity.Application.Models.Registration;
public class RegistrationModel : IMapFrom<RegistrationRequest>
{
    public string? Email { get; set; }

    public string? UserName { get; set; }

    public string Password { get; set; } = default!;

    public string ConfirmPassword { get; set; } = default!;

    public string? ReturnUrl { get; set; }
}
