using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.Registration;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Infrastructure.Persistence.Models;
using SFC.Identity.Application.Models.RefreshToken;
using SFC.Identity.Infrastructure.Extensions;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Common.Constants;

namespace SFC.Identity.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _signInManager = signInManager;
        }

        public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request)
        {
            if (!string.IsNullOrEmpty(request.UserName))
            {
                if (await _userManager.FindByNameAsync(request.UserName) != null)
                {
                    throw new ConflictException(Messages.UserAlreadyExist);
                }
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                if (await _userManager.FindByEmailAsync(request.Email) != null)
                {
                    throw new ConflictException(Messages.UserAlreadyExist);
                }
            }

            ApplicationUser user = new()
            {
                Email = request.Email,
                UserName = request.UserName,
                EmailConfirmed = true
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                AccessToken token = await CreateTokenAsync(user);

                return new RegistrationResponse
                {
                    UserId = user.Id,
                    Token = new JwtToken
                    {
                        Access = token.Value,
                        Refresh = token.RefreshToken.Value
                    }
                };
            }
            else
            {
                throw new IdentityException(Messages.UserRegistrationError,
                    result.Errors.ToDictionary(e => e.Code, e => new List<string> { e.Description }.AsEnumerable()));
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            ApplicationUser? user = null;

            if (!string.IsNullOrEmpty(request.UserName))
            {
                user = await _userManager.FindByNameAsync(request.UserName);
            }
            else if (!string.IsNullOrEmpty(request.Email))
            {
                user = await _userManager.FindByEmailAsync(request.Email);
            }

            if (user == null)
            {
                throw new AuthorizationException(Messages.AuthorizationError);
            }

            string username = (string.IsNullOrEmpty(request.UserName) ? request.Email : request.UserName) ?? string.Empty;

            SignInResult result = await _signInManager.PasswordSignInAsync(username, request.Password, false, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    throw new ForbiddenException(Messages.AccountLocked);
                }

                throw new AuthorizationException(Messages.AuthorizationError);
            }

            AccessToken token = await CreateTokenAsync(user);

            return new LoginResponse
            {
                UserId = user.Id,
                Token = new JwtToken
                {
                    Access = token.Value,
                    Refresh = token.RefreshToken.Value
                }
            };
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            ClaimsPrincipal principal = _jwtService.GetPrincipalFromExpiredToken(request.Token.Access)
                ?? throw new BadRequestException(Messages.ValidationError,
                        (nameof(request.Token.Access), Messages.TokenInvalid));

            ApplicationUser user = await _userManager.FindByNameAsync(principal.Identity?.Name ?? string.Empty)
                ?? throw new AuthorizationException(Messages.IncorrectTokenError);

            if (user.AccessToken == null
                || !user.AccessToken.RefreshToken.Value.Equals(request.Token.Refresh, StringComparison.InvariantCultureIgnoreCase)
                || user.AccessToken.RefreshToken.IsExpired)
            {
                throw new BadRequestException(Messages.ValidationError,
                    (nameof(request.Token.Refresh), Messages.TokenInvalid));
            }

            AccessToken token = await CreateTokenAsync(user);

            return new RefreshTokenResponse
            {
                Token = new JwtToken
                {
                    Access = token.Value,
                    Refresh = token.RefreshToken.Value
                }
            };
        }

        public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new NotFoundException(Messages.UserNotFound);

            await _signInManager.SignOutAsync();

            user.AccessToken = null;

            await _userManager.UpdateAsync(user);

            return new LogoutResponse();
        }

        private async Task<AccessToken> CreateTokenAsync(ApplicationUser user)
        {
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);

            userClaims.Add(new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty));

            AccessToken token = _jwtService.CreateAccessToken(userClaims);

            user.AccessToken = token;

            await _userManager.UpdateAsync(user);

            return token;
        }
    }
}
