using SFC.Identity.Application.Interfaces.Persistence.Context;
using SFC.Identity.Application.Interfaces.Persistence.Repository.Common;
using SFC.Identity.Domain.Entities.User;

namespace SFC.Identity.Application.Interfaces.Persistence.Repository.User;

public interface IUserRepository : IRepository<IUser, IIdentityDbContext, Guid>
{
    Task<IEnumerable<IUser>> GetByIdsAsync(IEnumerable<Guid> ids);
}