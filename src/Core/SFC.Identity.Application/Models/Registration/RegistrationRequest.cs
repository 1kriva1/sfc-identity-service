using SFC.Identity.Application.Common.Validators;
using System.ComponentModel.DataAnnotations;

namespace SFC.Identity.Application.Models.Registration
{
    [AtLeastOneRequired(nameof(Email), nameof(UserName))]
    public class RegistrationRequest
    {
        [EmailAddress(ErrorMessage = "EmailInvalid")]
        public string? Email { get; set; }

        [RegularExpression("^[a-zA-Z0-9-._@+]+$", ErrorMessage = "UserNameInvalid")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,}$",
            ErrorMessage = "PasswordInvalid")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "ConfirmPasswordRequired")]
        [Compare(nameof(Password), ErrorMessage = "ConfirmPasswordInvalid")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
