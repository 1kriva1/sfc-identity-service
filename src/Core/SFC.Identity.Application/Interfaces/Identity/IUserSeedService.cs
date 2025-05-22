using SFC.Identity.Domain.Entities.User;

namespace SFC.Identity.Application.Interfaces.Identity;
public interface IUserSeedService
{
    Task<IEnumerable<IUser>> GetSeedUsersAsync();

    Task SeedUsersAsync(CancellationToken cancellationToken);
}