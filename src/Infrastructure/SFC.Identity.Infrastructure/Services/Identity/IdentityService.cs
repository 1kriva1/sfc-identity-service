using AutoMapper;

using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using MassTransit;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Interfaces.Identity;
using SFC.Identity.Application.Interfaces.Identity.Dto.Base;
using SFC.Identity.Application.Interfaces.Identity.Dto.Login;
using SFC.Identity.Application.Interfaces.Identity.Dto.Logout;
using SFC.Identity.Application.Interfaces.Identity.Dto.Registration;
using SFC.Identity.Infrastructure.Persistence.Entities;
using SFC.Identity.Infrastructure.Settings;
using SFC.Identity.Messages.Events.User;

using LogoutRequest = Duende.IdentityServer.Models.LogoutRequest;


namespace SFC.Identity.Infrastructure.Services.Identity;

public record IdentityService(
    UserManager<ApplicationUser> UserManager,
    SignInManager<ApplicationUser> SignInManager,
    IIdentityServerInteractionService InteractionService,
    IServerUrls ServerUrls,
    IEventService Events,
    IPublishEndpoint PublishEndpoint,
    IOptions<IdentitySettings> IdentitySettings,
    IMapper Mapper) : IIdentityService
{
    #region Public

    public async Task<RegistrationResultDto> RegisterAsync(RegistrationModelDto model)
    {
        if (!string.IsNullOrEmpty(model.UserName))
        {
            if (await UserManager.FindByNameAsync(model.UserName).ConfigureAwait(true) != null)
            {
                throw new ConflictException(Localization.UserAlreadyExist);
            }
        }

        if (!string.IsNullOrEmpty(model.Email))
        {
            if (await UserManager.FindByEmailAsync(model.Email).ConfigureAwait(true) != null)
            {
                throw new ConflictException(Localization.UserAlreadyExist);
            }
        }

        ApplicationUser user = new()
        {
            Email = model.Email,
            UserName = model.UserName ?? model.Email,
            EmailConfirmed = true
        };

        IdentityResult createResult = await UserManager.CreateAsync(user, model.Password).ConfigureAwait(true);

        if (createResult.Succeeded)
        {
            SignInResult signInResult = await SignInManager.PasswordSignInAsync(user.UserName!, model.Password, false, lockoutOnFailure: true).ConfigureAwait(true);

            AuthorizationRequest? context = await GetAuthorizationRequest(model.ReturnUrl, out string? url).ConfigureAwait(true);

            if (!signInResult.Succeeded)
            {
                await Events.RaiseAsync(new UserLoginFailureEvent(model.UserName, Localization.AuthorizationError, clientId: context?.Client.ClientId)).ConfigureAwait(false);
                throw new AuthorizationException(Localization.AuthorizationError);
            }

            await Events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId)).ConfigureAwait(false);

            await PublishUserCreatedEventAsync(user).ConfigureAwait(false);

            return BuildBaseResult<RegistrationResultDto>(user, context, url);
        }
        else
        {
            throw new IdentityException(Localization.UserRegistrationError,
                createResult.Errors.ToDictionary(e => e.Code, e => new List<string> { e.Description }.AsEnumerable()));
        }
    }

    public async Task<LoginResultDto> LoginAsync(LoginModelDto model)
    {
        ApplicationUser? user = null;

        if (!string.IsNullOrEmpty(model.UserName))
        {
            user = await UserManager.FindByNameAsync(model.UserName).ConfigureAwait(true);
        }
        else if (!string.IsNullOrEmpty(model.Email))
        {
            user = await UserManager.FindByEmailAsync(model.Email).ConfigureAwait(true);
        }

        AuthorizationRequest? context = await GetAuthorizationRequest(model.ReturnUrl, out string? url).ConfigureAwait(true);

        if (user == null)
        {
            await Events.RaiseAsync(new UserLoginFailureEvent(model.UserName, Localization.AuthorizationError, clientId: context?.Client.ClientId)).ConfigureAwait(false);
            throw new AuthorizationException(Localization.AuthorizationError);
        }

        string username = (string.IsNullOrEmpty(model.UserName) ? model.Email : model.UserName) ?? string.Empty;

        SignInResult result = await SignInManager.PasswordSignInAsync(username, model.Password, model.RememberMe, lockoutOnFailure: true).ConfigureAwait(true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                await Events.RaiseAsync(new UserLoginFailureEvent(model.UserName, Localization.AccountLocked, clientId: context?.Client.ClientId)).ConfigureAwait(false);
                throw new ForbiddenException(Localization.AccountLocked);
            }

            await Events.RaiseAsync(new UserLoginFailureEvent(model.UserName, Localization.AuthorizationError, clientId: context?.Client.ClientId)).ConfigureAwait(false);
            throw new AuthorizationException(Localization.AuthorizationError);
        }

        await Events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId)).ConfigureAwait(false);

        return BuildBaseResult<LoginResultDto>(user, context, url, model.RememberMe);
    }

    public async Task<LogoutResultDto> LogoutAsync(LogoutModelDto model)
    {
        bool showLogoutPrompt = IdentitySettings.Value.Logout.ShowLogoutPrompt;

        if (!model.User.IsAuthenticated)
        {
            // if the user is not authenticated, then just show logged out page
            showLogoutPrompt = false;
        }
        else
        {
            LogoutRequest context = await InteractionService.GetLogoutContextAsync(model.LogoutId).ConfigureAwait(true);

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
            return await PostLogoutAsync(model).ConfigureAwait(true);
        }

        return new LogoutResultDto { ShowLogoutPrompt = showLogoutPrompt };
    }

    public async Task<LogoutResultDto> PostLogoutAsync(LogoutModelDto model)
    {
        LogoutRequest logoutRequest = await InteractionService.GetLogoutContextAsync(model.LogoutId).ConfigureAwait(true);

        await SignInManager.SignOutAsync().ConfigureAwait(true);

        // raise the logout event
        await Events.RaiseAsync(new UserLogoutSuccessEvent(model.User.Id, model.User.DisplayName)).ConfigureAwait(false);

        return new LogoutResultDto
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

    private T BuildBaseResult<T>(ApplicationUser user, AuthorizationRequest? context, string? url, bool rememberMe = false) where T : BaseResultDto, new()
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

    private Task PublishUserCreatedEventAsync(ApplicationUser user)
    {
        UserCreated @event = Mapper.Map<UserCreated>(user);
        return PublishEndpoint.Publish(@event);
    }

    #endregion Private
}