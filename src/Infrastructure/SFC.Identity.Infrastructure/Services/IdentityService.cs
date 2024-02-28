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

namespace SFC.Identity.Infrastructure.Services;

public record IdentityService(UserManager<ApplicationUser> UserManager,
    SignInManager<ApplicationUser> SignInManager,
    IJwtService JwtService) : IIdentityService
{
    public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request)
    {
        if (!string.IsNullOrEmpty(request.UserName))
        {
            if (await UserManager.FindByNameAsync(request.UserName) != null)
            {
                throw new ConflictException(Messages.UserAlreadyExist);
            }
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            if (await UserManager.FindByEmailAsync(request.Email) != null)
            {
                throw new ConflictException(Messages.UserAlreadyExist);
            }
        }

        ApplicationUser user = new()
        {
            Email = request.Email,
            UserName = request.UserName ?? request.Email,
            EmailConfirmed = true
        };

        IdentityResult result = await UserManager.CreateAsync(user, request.Password);

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
            user = await UserManager.FindByNameAsync(request.UserName);
        }
        else if (!string.IsNullOrEmpty(request.Email))
        {
            user = await UserManager.FindByEmailAsync(request.Email);
        }

        if (user == null)
        {
            throw new AuthorizationException(Messages.AuthorizationError);
        }

        string username = (string.IsNullOrEmpty(request.UserName) ? request.Email : request.UserName) ?? string.Empty;

        SignInResult result = await SignInManager.PasswordSignInAsync(username, request.Password, false, lockoutOnFailure: true);

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
        ClaimsPrincipal principal = JwtService.GetPrincipalFromExpiredToken(request.Token.Access)
            ?? throw new BadRequestException(Messages.ValidationError,
                    (nameof(request.Token.Access), Messages.TokenInvalid));

        Claim? userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new BadRequestException(Messages.ValidationError,
                    (nameof(request.Token.Access), Messages.TokenInvalid));

        ApplicationUser user = await UserManager.FindByIdAsync(userIdClaim.Value)
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
        ApplicationUser? user = await UserManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException(Messages.UserNotFound);

        await SignInManager.SignOutAsync();

        user.AccessToken = null;

        await UserManager.UpdateAsync(user);

        return new LogoutResponse();
    }

    private async Task<AccessToken> CreateTokenAsync(ApplicationUser user)
    {
        IList<Claim> userClaims = await UserManager.GetClaimsAsync(user);

        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

        AccessToken token = JwtService.CreateAccessToken(userClaims);

        user.AccessToken = token;

        await UserManager.UpdateAsync(user);

        return token;
    }
}
