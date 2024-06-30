using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Existence;

namespace SFC.Identity.Api.Controllers;

[AllowAnonymous]
[ProducesResponseType(StatusCodes.Status200OK)]
public class ExistenceController(IExistenceService existenceService) : ApiControllerBase
{
    /// <summary>
    /// Check user existence by name.
    /// </summary>
    /// <param name="userName">User name</param>
    /// <returns>An ActionResult of type ExistenceResponse</returns>
    /// <response code="200">Returns user existence check result by **name**.</response>
    [HttpGet("name/{userName}")]

    public async Task<ActionResult<ExistenceResponse>> CheckExistenceByUserNameAsync([FromRoute] string userName)
    {
        ExistenceResponse response = await existenceService.CheckByUserNameAsync(userName);

        return Ok(response);
    }

    /// <summary>
    /// Check user existence by email.
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>An ActionResult of type ExistenceResponse</returns>
    /// <response code="200">Returns user existence check result by **email**.</response>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<ExistenceResponse>> CheckExistenceByEmailAsync([FromRoute] string email)
    {
        ExistenceResponse response = await existenceService.CheckByEmailAsync(email);

        return Ok(response);
    }
}
