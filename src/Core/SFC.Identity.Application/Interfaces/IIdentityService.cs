using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.RefreshToken;
using SFC.Identity.Application.Models.Registration;

namespace SFC.Identity.Application.Interfaces;

public interface IIdentityService
{
    Task<RegistrationResponse> RegisterAsync(RegistrationRequest request);

    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task<LogoutResponse> LogoutAsync(LogoutRequest request);

    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
}
