namespace SFC.Identity.Application.Interfaces.Identity.Dto.Logout;
public class LogoutModelDto
{
    public string LogoutId { get; set; } = default!;

    public LogoutUserModelDto User { get; set; } = default!;
}

public class LogoutUserModelDto
{
    public bool IsAuthenticated { get; set; }

    public string Id { get; set; } = default!;

    public string DisplayName { get; set; } = default!;
}