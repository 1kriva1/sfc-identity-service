using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace SFC.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private IMapper? _mapper;

    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();
}
