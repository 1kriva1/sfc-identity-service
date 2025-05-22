using Microsoft.EntityFrameworkCore;

using SFC.Identity.Application.Features.Common.Models.Find;
using SFC.Identity.Application.Features.Common.Models.Find.Paging;
using SFC.Identity.Application.Interfaces.Persistence.Repository.User;
using SFC.Identity.Domain.Entities.User;
using SFC.Identity.Infrastructure.Persistence.Contexts;
using SFC.Identity.Infrastructure.Persistence.Entities;
using SFC.Identity.Infrastructure.Persistence.Extensions;
using SFC.Identity.Infrastructure.Persistence.Repositories.Common;

namespace SFC.Identity.Infrastructure.Persistence.Repositories.User;

public class UserRepository(IdentityDbContext context) : Repository<IUser, IdentityDbContext, Guid>(context), IUserRepository
{
    #region Overrides

    public override async Task<IUser?> GetByIdAsync(Guid id)
    {
        IUser? entity = await Context.Users.FindAsync(id).ConfigureAwait(true);
        return entity;
    }

    public override Task<PagedList<IUser>> FindAsync(FindParameters<IUser> parameters)
    {
        return Context.Users
                      .AsQueryable<IUser>()
                      .PaginateAsync(parameters);
    }

    public override async Task<IReadOnlyList<IUser>> ListAllAsync()
    {
        return await Context.Users.ToListAsync().ConfigureAwait(true);
    }

    public override async Task<IUser> AddAsync(IUser entity)
    {
        await Context.Users.AddAsync((ApplicationUser)entity).ConfigureAwait(true);

        await Context.SaveChangesAsync().ConfigureAwait(true);

        return entity;
    }

    public override async Task<IUser[]> AddRangeAsync(params IUser[] entities)
    {
        IEnumerable<ApplicationUser> applicationUsers = entities.Select(entity => (ApplicationUser)entity);

        await Context.Users.AddRangeAsync(applicationUsers).ConfigureAwait(true);

        await Context.SaveChangesAsync().ConfigureAwait(true);

        return entities;
    }

    public override Task UpdateAsync(IUser entity)
    {
        Context.Entry(entity).State = EntityState.Modified;

        return Context.SaveChangesAsync();
    }

    public override Task DeleteAsync(IUser entity)
    {
        Context.Users.Remove((ApplicationUser)entity);

        return Context.SaveChangesAsync();
    }

    #endregion Overrides

    #region Public

    public async Task<IEnumerable<IUser>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await Context.Users
                            .Where(user => ids.Contains(user.Id))
                            .ToListAsync()
                            .ConfigureAwait(true);
    }

    #endregion Public
}