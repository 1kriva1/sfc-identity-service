namespace SFC.Identity.Application.Interfaces.Existence;

public interface IExistenceService
{
    Task<bool> CheckByUserNameAsync(string userName);

    Task<bool> CheckByEmailAsync(string email);
}