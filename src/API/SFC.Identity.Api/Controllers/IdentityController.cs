using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Base;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.RefreshToken;
using SFC.Identity.Application.Models.Registration;

namespace SFC.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
public class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    /// <summary>
    /// Register new user.
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <returns>An ActionResult of type RegistrationResponse</returns>
    /// <response code="200">Returns **new** created user id with linked JWT token.</response>
    /// <response code="400">Returns **validation** errors.</response>
    /// <response code="409">Returns if user **already exist** by name or email.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegistrationResponse>> RegisterAsync([FromBody] RegistrationRequest request)
    {
        RegistrationResponse response = await _identityService.RegisterAsync(request);

        return Ok(response);
    }

    /// <summary>
    /// Login user.
    /// </summary>
    /// <param name="request">Login request</param>
    /// <returns>An ActionResult of type LoginResponse</returns>
    /// <response code="200">Returns **successfully** authenticated user with id and linked JWT token.</response>
    /// <response code="400">Returns **validation** errors.</response>
    /// <response code="401">Returns **failed** authentication result.</response>
    /// <response code="403">Returns when user is **locked**.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
    {
        LoginResponse response = await _identityService.LoginAsync(request);

        return Ok(response);
    }

    /// <summary>
    /// Logout user.
    /// </summary>
    /// <param name="request">Logout request</param>
    /// <returns>An ActionResult of type LogoutResponse</returns>
    /// <response code="200">Returns **successfully** logout user result.</response>
    /// <response code="400">Returns **validation** errors.</response>
    /// <response code="404">Returns when user **not found** by id.</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LogoutResponse>> LogoutAsync([FromBody] LogoutRequest request)
    {
        LogoutResponse response = await _identityService.LogoutAsync(request);

        return Ok(response);
    }

    /// <summary>
    /// Refresh JWT token.
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>An ActionResult of type RefreshTokenResponse</returns>
    /// <response code="200">Returns **successfully** refreshed JWT token.</response>
    /// <response code="400">Returns **validation** error, when access or refresh tokens are invalid.</response>
    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        RefreshTokenResponse response = await _identityService.RefreshTokenAsync(request);

        return Ok(response);
    }
}
