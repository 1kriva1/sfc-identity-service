using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.Models.Login
{
    public class LogoutRequest
    {
        [Required]
        public string UserId { get; set; } = null!;
    }
}
