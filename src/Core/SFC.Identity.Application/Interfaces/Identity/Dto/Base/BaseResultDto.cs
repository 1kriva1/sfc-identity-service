namespace SFC.Identity.Application.Interfaces.Identity.Dto.Base;
public class BaseResultDto
{
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string ReturnUrl { get; set; } = default!;
#pragma warning restore CA1056 // URI-like properties should not be strings

    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public object Properties { get; set; } = default!;
}