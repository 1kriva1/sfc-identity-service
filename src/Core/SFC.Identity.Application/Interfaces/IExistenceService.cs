using SFC.Identity.Application.Models.Existence;

namespace SFC.Identity.Application.Interfaces;

public interface IExistenceService
{
    Task<ExistenceResponse> CheckByUserNameAsync(string userName);

    Task<ExistenceResponse> CheckByEmailAsync(string email);
}
