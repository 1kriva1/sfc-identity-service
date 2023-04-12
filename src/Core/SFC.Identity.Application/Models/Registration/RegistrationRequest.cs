using SFC.Identity.Application.Common.Validators;
using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.Models.Registration
{
    [AtLeastOneRequired(nameof(Email), nameof(UserName))]
    public class RegistrationRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        [RegularExpression("^[a-zA-Z0-9-._@+]+$")]
        public string? UserName { get; set; }

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,}$",
            ErrorMessage = "Passwords must have at least 6 characters, " +
            "one non alphanumeric character, " +
            "one digit ('0'-'9'), " +
            "one uppercase ('A'-'Z'), " +
            "one lowercase ('a'-'z').")]
        public string Password { get; set; } = null!;

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;
    }
}
