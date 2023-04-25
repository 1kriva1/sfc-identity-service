using Microsoft.AspNetCore.Identity;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Application.Models.Existence;
using SFC.Identity.Infrastructure.Persistence.Models;

namespace SFC.Identity.Infrastructure.Services
{
    public class ExistenceService : IExistenceService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ExistenceService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ExistenceResponse> CheckByUserNameAsync(string userName)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(userName);

            return new ExistenceResponse { Exist = user != null };
        }

        public async Task<ExistenceResponse> CheckByEmailAsync(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            return new ExistenceResponse { Exist = user != null };
        }
    }
}
