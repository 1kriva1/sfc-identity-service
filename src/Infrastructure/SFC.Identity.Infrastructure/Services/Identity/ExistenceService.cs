using Microsoft.AspNetCore.Identity;

using SFC.Identity.Application.Interfaces.Existence;
using SFC.Identity.Infrastructure.Persistence.Entities;

namespace SFC.Identity.Infrastructure.Services.Identity;

public record ExistenceService(UserManager<ApplicationUser> UserManager) : IExistenceService
{
    public async Task<bool> CheckByUserNameAsync(string userName)
    {
        ApplicationUser? user = await UserManager.FindByNameAsync(userName).ConfigureAwait(true);
        return user is not null;
    }

    public async Task<bool> CheckByEmailAsync(string email)
    {
        ApplicationUser? user = await UserManager.FindByEmailAsync(email).ConfigureAwait(true);
        return user is not null;
    }
}