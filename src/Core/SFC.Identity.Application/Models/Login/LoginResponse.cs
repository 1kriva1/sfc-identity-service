using SFC.Identity.Application.Common.Models;

namespace SFC.Identity.Application.Models.Login
{
    public class LoginResponse: BaseResponse
    {
        public Guid UserId { get; set; }

        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
