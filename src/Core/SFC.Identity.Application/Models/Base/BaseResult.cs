namespace SFC.Identity.Application.Models.Base;
public class BaseResult
{
    public string ReturnUrl { get; set; } = default!;

    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public object Properties { get; set; } = default!;
}
