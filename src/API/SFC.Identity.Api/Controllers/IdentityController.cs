using System.ComponentModel.DataAnnotations;

using Duende.IdentityServer;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SFC.Identity.Api.Extensions;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Base;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.Logout;
using SFC.Identity.Application.Models.Registration;

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
        RegistrationModel model = Mapper.Map<RegistrationModel>(request);

        RegistrationResult result = await identityService.RegisterAsync(model);

        return await BuildResponse<RegistrationResponse>(result);
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
        LoginModel model = Mapper.Map<LoginModel>(request);

        LoginResult result = await identityService.LoginAsync(model);

        return await BuildResponse<LoginResponse>(result);
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
        LogoutModel model = User.BuildLogoutModel(logoutId);

        LogoutResult result = await identityService.LogoutAsync(model);

        if (!result.ShowLogoutPrompt)
        {
            await SignOutAsync();
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
        [FromQuery] [Required(ErrorMessage = "LogoutIdRequired")] string logoutId)
    {
        LogoutModel model = User.BuildLogoutModel(logoutId);

        LogoutResult result = await identityService.PostLogoutAsync(model);

        await SignOutAsync();

        return Ok(Mapper.Map<LogoutResponse>(result));
    }

    #endregion Public

    #region Private

    private async Task<ActionResult<T>> BuildResponse<T>(BaseResult result)
    {
        IdentityServerUser user = new(result.UserId.ToString()) { DisplayName = result.UserName };

        await HttpContext.SignInAsync(user.CreatePrincipal(), (AuthenticationProperties)result.Properties);

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
