using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.Models.RefreshToken
{
    public class RefreshTokenRequest
    {
        [Required]
        public string AccessToken { get; set; } = null!;

        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
