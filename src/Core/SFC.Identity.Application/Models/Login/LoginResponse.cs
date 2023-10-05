using SFC.Identity.Application.Common.Models;
using SFC.Identity.Application.Models.Tokens;

namespace SFC.Identity.Application.Models.Login;

public class LoginResponse: BaseResponse
{
    public Guid UserId { get; set; }

    public JwtToken Token { get; set; } = null!;
}
