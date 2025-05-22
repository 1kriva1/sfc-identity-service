namespace SFC.Identity.Application.Interfaces.Identity.Dto.Login;
public class LoginModelDto
{
    public string? Email { get; set; }

    public string? UserName { get; set; }

    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? ReturnUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings
}