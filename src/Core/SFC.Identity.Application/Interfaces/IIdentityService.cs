using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.Logout;
using SFC.Identity.Application.Models.Registration;

namespace SFC.Identity.Application.Interfaces;

public interface IIdentityService
{
    Task<RegistrationResult> RegisterAsync(RegistrationModel model);

    Task<LoginResult> LoginAsync(LoginModel model);

    Task<LogoutResult> LogoutAsync(LogoutModel model);

    Task<LogoutResult> PostLogoutAsync(LogoutModel model);
}
