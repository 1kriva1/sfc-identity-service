using SFC.Identity.Application.Common.Mappings;
using SFC.Identity.Application.Models.Login;

namespace SFC.Identity.Application.Models.Logout;
public class LogoutModel
{
    public string LogoutId { get; set; } = default!;

    public LogoutUserModel User { get; set; } = default!;
}

public class LogoutUserModel
{
    public bool IsAuthenticated { get; set; }

    public string Id { get; set; } = default!;

    public string DisplayName { get; set; } = default!;
}
