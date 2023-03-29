using SFC.Identity.Application.Models.Existence;

namespace SFC.Identity.Application.Interfaces
{
    public interface IExistenceService
    {
        Task<ExistenceResponse> CheckByUserName(string userName);

        Task<ExistenceResponse> CheckByEmail(string email);
    }
}
