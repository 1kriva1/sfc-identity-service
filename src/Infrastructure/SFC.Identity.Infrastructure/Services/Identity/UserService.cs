using Microsoft.AspNetCore.Http;

using SFC.Identity.Application.Interfaces.Identity;
using SFC.Identity.Infrastructure.Extensions;

namespace SFC.Identity.Infrastructure.Services.Identity;
public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? GetUserId() => _httpContextAccessor.GetUserId();
}