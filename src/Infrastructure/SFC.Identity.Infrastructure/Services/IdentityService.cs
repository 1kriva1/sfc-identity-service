using Microsoft.AspNetCore.Identity;
using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.Registration;
using SFC.Identity.Infrastructure.Persistence.Models;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Common.Constants;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Duende.IdentityServer.Events;
using Microsoft.Extensions.Options;
using SFC.Identity.Infrastructure.Settings;
using SFC.Identity.Application.Models.Logout;
using SFC.Identity.Application.Models.Base;

using LogoutRequest = Duende.IdentityServer.Models.LogoutRequest;

namespace SFC.Identity.Infrastructure.Services;

public record IdentityService(
    UserManager<ApplicationUser> UserManager,
    SignInManager<ApplicationUser> SignInManager,
    IIdentityServerInteractionService InteractionService,
    IServerUrls ServerUrls,
    IEventService Events,
    IOptions<IdentitySettings> IdentitySettings) : IIdentityService
{
    #region Public

    public async Task<RegistrationResult> RegisterAsync(RegistrationModel model)
    {
        if (!string.IsNullOrEmpty(model.UserName))
        {
            if (await UserManager.FindByNameAsync(model.UserName) != null)
            {
                throw new ConflictException(Messages.UserAlreadyExist);
            }
        }

        if (!string.IsNullOrEmpty(model.Email))
        {
            if (await UserManager.FindByEmailAsync(model.Email) != null)
            {
                throw new ConflictException(Messages.UserAlreadyExist);
            }
        }

        ApplicationUser user = new()
        {
            Email = model.Email,
            UserName = model.UserName ?? model.Email,
            EmailConfirmed = true
        };

        IdentityResult createResult = await UserManager.CreateAsync(user, model.Password);

        if (createResult.Succeeded)
        {
            SignInResult signInResult = await SignInManager.PasswordSignInAsync(user.UserName!, model.Password, false, lockoutOnFailure: true);

            AuthorizationRequest? context = await GetAuthorizationRequest(model.ReturnUrl, out string? url);

            if (!signInResult.Succeeded)
            {
                await Events.RaiseAsync(new UserLoginFailureEvent(model.UserName, Messages.AuthorizationError, clientId: context?.Client.ClientId));
                throw new AuthorizationException(Messages.AuthorizationError);
            }

            await Events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));

            return BuildBaseResult<RegistrationResult>(user, context, url);
        }
        else
        {
            throw new IdentityException(Messages.UserRegistrationError,
                createResult.Errors.ToDictionary(e => e.Code, e => new List<string> { e.Description }.AsEnumerable()));
        }
    }

    public async Task<LoginResult> LoginAsync(LoginModel model)
    {
        ApplicationUser? user = null;

        if (!string.IsNullOrEmpty(model.UserName))
        {
            user = await UserManager.FindByNameAsync(model.UserName);
        }
        else if (!string.IsNullOrEmpty(model.Email))
        {
            user = await UserManager.FindByEmailAsync(model.Email);
        }

        AuthorizationRequest? context = await GetAuthorizationRequest(model.ReturnUrl, out string? url);

        if (user == null)
        {
            await Events.RaiseAsync(new UserLoginFailureEvent(model.UserName, Messages.AuthorizationError, clientId: context?.Client.ClientId));
            throw new AuthorizationException(Messages.AuthorizationError);
        }

        string username = (string.IsNullOrEmpty(model.UserName) ? model.Email : model.UserName) ?? string.Empty;

        SignInResult result = await SignInManager.PasswordSignInAsync(username, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                await Events.RaiseAsync(new UserLoginFailureEvent(model.UserName, Messages.AccountLocked, clientId: context?.Client.ClientId));
                throw new ForbiddenException(Messages.AccountLocked);
            }

            await Events.RaiseAsync(new UserLoginFailureEvent(model.UserName, Messages.AuthorizationError, clientId: context?.Client.ClientId));
            throw new AuthorizationException(Messages.AuthorizationError);
        }

        await Events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));

        return BuildBaseResult<LoginResult>(user, context, url, model.RememberMe);
    }

    public async Task<LogoutResult> LogoutAsync(LogoutModel model)
    {
        bool showLogoutPrompt = IdentitySettings.Value.Logout.ShowLogoutPrompt;

        if (!model.User.IsAuthenticated)
        {
            // if the user is not authenticated, then just show logged out page
            showLogoutPrompt = false;
        }
        else
        {
            LogoutRequest context = await InteractionService.GetLogoutContextAsync(model.LogoutId);

            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                showLogoutPrompt = false;
            }
        }

        if (!showLogoutPrompt)
        {
            // if the request for logout was properly authenticated from IdentityServer, then
            // we don't need to show the prompt and can just log the user out directly.
            return await PostLogoutAsync(model);
        }

        return new LogoutResult { ShowLogoutPrompt = showLogoutPrompt };
    }

    public async Task<LogoutResult> PostLogoutAsync(LogoutModel model)
    {
        LogoutRequest logoutRequest = await InteractionService.GetLogoutContextAsync(model.LogoutId);

        await SignInManager.SignOutAsync();

        // raise the logout event
        await Events.RaiseAsync(new UserLogoutSuccessEvent(model.User.Id, model.User.DisplayName));

        return new LogoutResult
        {
            AutomaticRedirectAfterSignOut = IdentitySettings.Value.Logout.AutomaticRedirectAfterSignOut,
            PostLogoutRedirectUrl = logoutRequest?.PostLogoutRedirectUri,
            ClientName = string.IsNullOrEmpty(logoutRequest?.ClientName) ? logoutRequest?.ClientId : logoutRequest?.ClientName,
            SignOutIFrameUrl = logoutRequest?.SignOutIFrameUrl
        };
    }

    #endregion Public

    #region Private

    private Task<AuthorizationRequest?> GetAuthorizationRequest(string? returnUrl, out string? url)
    {
        url = returnUrl != null ? Uri.UnescapeDataString(returnUrl) : null;

        return InteractionService.GetAuthorizationContextAsync(url);
    }

    private T BuildBaseResult<T>(ApplicationUser user, AuthorizationRequest? context, string? url, bool rememberMe = false) where T : BaseResult, new()
    {
        AuthenticationProperties properties = new()
        {
            IsPersistent = IdentitySettings.Value.Login.AllowRememberLogin && rememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(IdentitySettings.Value.Login.RememberLoginDuration))
        };

        return new T
        {
            ReturnUrl = context != null ? url! : ServerUrls.BaseUrl,
            UserId = user.Id,
            UserName = user.UserName,
            Properties = properties
        };
    }

    #endregion Private
}
