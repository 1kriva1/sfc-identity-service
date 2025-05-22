using Microsoft.EntityFrameworkCore;

using SFC.Identity.Application.Features.Common.Models.Find;
using SFC.Identity.Application.Features.Common.Models.Find.Paging;
using SFC.Identity.Application.Interfaces.Persistence.Context;
using SFC.Identity.Application.Interfaces.Persistence.Repository.Common;
using SFC.Identity.Infrastructure.Persistence.Extensions;

namespace SFC.Identity.Infrastructure.Persistence.Repositories.Common;

public class Repository<TEntity, TContext, TId>(TContext context) : IRepository<TEntity, TContext, TId>
    where TEntity : class
    where TContext : DbContext, IDbContext
{
    protected TContext Context { get; } = context;

    public virtual async Task<TEntity?> GetByIdAsync(TId id)
    {
        TEntity? entity = await Context.Set<TEntity>().FindAsync(id).ConfigureAwait(true);
        return entity;
    }

    public virtual Task<PagedList<TEntity>> FindAsync(FindParameters<TEntity> parameters)
    {
        return Context.Set<TEntity>()
                      .AsQueryable<TEntity>()
                      .PaginateAsync(parameters);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAllAsync()
    {
        return await Context.Set<TEntity>().ToListAsync().ConfigureAwait(true);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await Context.Set<TEntity>().AddAsync(entity).ConfigureAwait(true);

        await Context.SaveChangesAsync().ConfigureAwait(true);

        return entity;
    }

    public virtual async Task<TEntity[]> AddRangeAsync(params TEntity[] entities)
    {
        await Context.Set<TEntity>().AddRangeAsync(entities).ConfigureAwait(true);

        await Context.SaveChangesAsync().ConfigureAwait(true);

        return entities;
    }

    public virtual Task UpdateAsync(TEntity entity)
    {
        Context.Entry(entity).State = EntityState.Modified;

        return Context.SaveChangesAsync();
    }

    public virtual Task DeleteAsync(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);

        return Context.SaveChangesAsync();
    }
}