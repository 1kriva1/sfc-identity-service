using SFC.Identity.Application.Common.Validators;
using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.Models.Login
{
    [AtLeastOneRequired(nameof(Email), nameof(UserName))]
    public class LoginRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? UserName { get; set; }

        [Required]
        public string Password { get; set; } = null!;
    }
}
