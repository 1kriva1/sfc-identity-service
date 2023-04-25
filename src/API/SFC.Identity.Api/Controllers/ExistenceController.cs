using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Existence;

namespace SFC.Identity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ExistenceController : ControllerBase
    {
        private readonly IExistenceService _existenceService;

        public ExistenceController(IExistenceService existenceService)
        {
            _existenceService = existenceService;
        }

        [HttpGet("name/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExistenceResponse>> CheckExistenceByUserName([FromRoute] string userName)
        {
            ExistenceResponse response = await _existenceService.CheckByUserNameAsync(userName);

            return Ok(response);
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExistenceResponse>> CheckExistenceByEmail([FromRoute] string email)
        {
            ExistenceResponse response = await _existenceService.CheckByEmailAsync(email);

            return Ok(response);
        }
    }
}
