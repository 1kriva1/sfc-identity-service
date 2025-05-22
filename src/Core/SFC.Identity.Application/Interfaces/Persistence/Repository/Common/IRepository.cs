using SFC.Identity.Application.Features.Common.Models.Find;
using SFC.Identity.Application.Features.Common.Models.Find.Paging;
using SFC.Identity.Application.Interfaces.Persistence.Context;

namespace SFC.Identity.Application.Interfaces.Persistence.Repository.Common;

public interface IRepository<TEntity, TContext, TId>
    where TEntity : class
    where TContext : IDbContext
{
    Task<TEntity?> GetByIdAsync(TId id);

    Task<PagedList<TEntity>> FindAsync(FindParameters<TEntity> parameters);

    Task<IReadOnlyList<TEntity>> ListAllAsync();

    Task<TEntity> AddAsync(TEntity entity);

    Task<TEntity[]> AddRangeAsync(params TEntity[] entities);

    Task UpdateAsync(TEntity entity);

    Task DeleteAsync(TEntity entity);
}