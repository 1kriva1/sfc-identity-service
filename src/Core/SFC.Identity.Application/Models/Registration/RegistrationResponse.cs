using SFC.Identity.Application.Models.Base;
using SFC.Identity.Application.Models.Tokens;

namespace SFC.Identity.Application.Models.Registration;

public class RegistrationResponse : BaseResponse
{
    public Guid UserId { get; set; }

    public JwtToken Token { get; set; } = null!;
}
