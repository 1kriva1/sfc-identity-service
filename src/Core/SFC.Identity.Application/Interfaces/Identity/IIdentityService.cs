using SFC.Identity.Application.Interfaces.Identity.Dto.Login;
using SFC.Identity.Application.Interfaces.Identity.Dto.Logout;
using SFC.Identity.Application.Interfaces.Identity.Dto.Registration;

namespace SFC.Identity.Application.Interfaces.Identity;
public interface IIdentityService
{
    Task<RegistrationResultDto> RegisterAsync(RegistrationModelDto model);

    Task<LoginResultDto> LoginAsync(LoginModelDto model);

    Task<LogoutResultDto> LogoutAsync(LogoutModelDto model);

    Task<LogoutResultDto> PostLogoutAsync(LogoutModelDto model);
}