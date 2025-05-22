using System.ComponentModel.DataAnnotations;

using Duende.IdentityServer;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SFC.Identity.Api.Infrastructure.Models.Base;
using SFC.Identity.Api.Infrastructure.Models.Identity.Login;
using SFC.Identity.Api.Infrastructure.Models.Identity.Logout;
using SFC.Identity.Api.Infrastructure.Models.Identity.Registration;
using SFC.Identity.Application.Interfaces.Identity;
using SFC.Identity.Application.Interfaces.Identity.Dto.Base;
using SFC.Identity.Application.Interfaces.Identity.Dto.Login;
using SFC.Identity.Application.Interfaces.Identity.Dto.Logout;
using SFC.Identity.Application.Interfaces.Identity.Dto.Registration;
using SFC.Identity.Infrastructure.Extensions;

namespace SFC.Identity.Api.Controllers;

[AllowAnonymous]
[ProducesResponseType(StatusCodes.Status200OK)]
public class IdentityController(IIdentityService identityService) : ApiControllerBase
{
    #region Public

    /// <summary>
    /// Register new user.
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <returns>An ActionResult of type RegistrationResponse</returns>
    /// <response code="200">Returns when **new** user was created.</response>
    /// <response code="400">Returns **validation** errors.</response>
    /// <response code="409">Returns if user **already exist** by name or email.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegistrationResponse>> RegisterAsync([FromBody] RegistrationRequest request)
    {
        RegistrationModelDto model = Mapper.Map<RegistrationModelDto>(request);

        RegistrationResultDto result = await identityService.RegisterAsync(model).ConfigureAwait(true);

        return await BuildResponseAsync<RegistrationResponse>(result).ConfigureAwait(true);
    }

    /// <summary>
    /// Login user.
    /// </summary>
    /// <param name="request">Login request</param>
    /// <returns>An ActionResult of type LoginResponse</returns>
    /// <response code="200">Returns when user was **successfully** authenticated.</response>
    /// <response code="400">Returns **validation** errors.</response>
    /// <response code="401">Returns **failed** authentication result.</response>
    /// <response code="403">Returns when user is **locked**.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
    {
        LoginModelDto model = Mapper.Map<LoginModelDto>(request);

        LoginResultDto result = await identityService.LoginAsync(model).ConfigureAwait(true);

        return await BuildResponseAsync<LoginResponse>(result).ConfigureAwait(true);
    }

    /// <summary>
    /// Logout user.
    /// </summary>
    /// <param name="logoutId">Logout identificator</param>
    /// <returns>An ActionResult of type LogoutResponse</returns>
    /// <response code="200">Returns **successfully** logout user result.</response>
    [HttpGet("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<LogoutResponse>> LogoutAsync(
        [FromQuery][Required(ErrorMessage = "LogoutIdRequired")] string logoutId)
    {
        LogoutModelDto model = User.BuildLogoutModel(logoutId);

        LogoutResultDto result = await identityService.LogoutAsync(model).ConfigureAwait(true);

        if (!result.ShowLogoutPrompt)
        {
            await SignOutAsync().ConfigureAwait(true);
        }

        return Ok(Mapper.Map<LogoutResponse>(result));
    }


    /// <summary>
    /// Logout user after confirmation.
    /// </summary>
    /// <param name="logoutId">Logout identificator</param>
    /// <returns>An ActionResult of type LogoutResponse</returns>
    /// <response code="200">Returns **successfully** logout user result.</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<LogoutResponse>> PostLogoutAsync(
        [FromQuery][Required(ErrorMessage = "LogoutIdRequired")] string logoutId)
    {
        LogoutModelDto model = User.BuildLogoutModel(logoutId);

        LogoutResultDto result = await identityService.PostLogoutAsync(model).ConfigureAwait(true);

        await SignOutAsync().ConfigureAwait(true);

        return Ok(Mapper.Map<LogoutResponse>(result));
    }

    #endregion Public

    #region Private

    private async Task<ActionResult<T>> BuildResponseAsync<T>(BaseResultDto result)
    {
        IdentityServerUser user = new(result.UserId.ToString()) { DisplayName = result.UserName };

        await HttpContext.SignInAsync(user.CreatePrincipal(), (AuthenticationProperties)result.Properties).ConfigureAwait(true);

        return Ok(Mapper.Map<T>(result));
    }

    private Task SignOutAsync()
    {
        // delete local authentication cookie
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // delete openid authentication cookie
        HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

        return Task.CompletedTask;
    }

    #endregion Private
}