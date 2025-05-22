namespace SFC.Identity.Application.Interfaces.Identity.Dto.Registration;
public class RegistrationModelDto
{
    public string? Email { get; set; }

    public string? UserName { get; set; }

    public string Password { get; set; } = default!;

    public string ConfirmPassword { get; set; } = default!;

#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? ReturnUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings
}