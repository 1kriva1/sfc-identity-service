using SFC.Identity.Application.Common.Models;

namespace SFC.Identity.Application.Models.RefreshToken
{
    public class RefreshTokenResponse: BaseResponse
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
