using Microsoft.AspNetCore.Identity;

using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Existence;
using SFC.Identity.Infrastructure.Persistence.Models;

namespace SFC.Identity.Infrastructure.Services;

public record ExistenceService(UserManager<ApplicationUser> UserManager) : IExistenceService
{
    public async Task<ExistenceResponse> CheckByUserNameAsync(string userName)
    {
        ApplicationUser? user = await UserManager.FindByNameAsync(userName);

        return new ExistenceResponse { Exist = user is not null };
    }

    public async Task<ExistenceResponse> CheckByEmailAsync(string email)
    {
        ApplicationUser? user = await UserManager.FindByEmailAsync(email);

        return new ExistenceResponse { Exist = user is not null };
    }
}
