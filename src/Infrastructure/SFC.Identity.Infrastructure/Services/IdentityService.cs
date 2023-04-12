using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.Registration;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Infrastructure.Persistence.Models;
using SFC.Identity.Application.Models.RefreshToken;
using SFC.Identity.Infrastructure.Extensions;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Interfaces;

namespace SFC.Identity.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private const string AUTHORIZATION_ERROR_MESSAGE = "User not found or incorrect password.";

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
                    throw new ConflictException($"User already exists.");
                }
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                if (await _userManager.FindByEmailAsync(request.Email) != null)
                {
                    throw new ConflictException($"User already exists.");
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
                throw new IdentityException("Error occured during user registration process.",
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
                throw new AuthorizationException(AUTHORIZATION_ERROR_MESSAGE);
            }

            string username = (string.IsNullOrEmpty(request.UserName) ? request.Email : request.UserName) ?? string.Empty;

            SignInResult result = await _signInManager.PasswordSignInAsync(username, request.Password, false, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    throw new ForbiddenException("User account locked out.");
                }

                throw new AuthorizationException(AUTHORIZATION_ERROR_MESSAGE);
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
                ?? throw new BadRequestException(ErrorConstants.VALIDATION_ERROR_MESSAGE, (nameof(request.Token.Access), ErrorConstants.INVALID_TOKEN_ERROR_MESSAGE));

            ApplicationUser user = await _userManager.FindByNameAsync(principal.Identity?.Name ?? string.Empty)
                ?? throw new AuthorizationException("User not found or incorrect token.");

            if (user.AccessToken == null
                || !user.AccessToken.RefreshToken.Value.Equals(request.Token.Refresh, StringComparison.InvariantCultureIgnoreCase)
                || user.AccessToken.RefreshToken.IsExpired)
            {
                throw new BadRequestException(ErrorConstants.VALIDATION_ERROR_MESSAGE, (nameof(request.Token.Refresh), ErrorConstants.INVALID_TOKEN_ERROR_MESSAGE));
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
                ?? throw new NotFoundException("User not found.");

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
