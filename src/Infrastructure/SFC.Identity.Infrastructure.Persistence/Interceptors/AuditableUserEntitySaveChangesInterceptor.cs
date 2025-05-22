using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using SFC.Identity.Application.Interfaces.Common;
using SFC.Identity.Domain.Common.Interfaces;
using SFC.Identity.Infrastructure.Persistence.Extensions;

namespace SFC.Identity.Infrastructure.Persistence.Interceptors;
public class AuditableUserEntitySaveChangesInterceptor(IDateTimeService dateTimeService) : SaveChangesInterceptor
{
    private readonly IDateTimeService _dateTimeService = dateTimeService;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        IEnumerable<EntityEntry<IAuditableUserEntity>> entries = context.ChangeTracker.Entries<IAuditableUserEntity>();

        foreach (EntityEntry<IAuditableUserEntity> entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedBy == Guid.Empty)
                    entry.Entity.CreatedBy = entry.Entity.Id;

                entry.Entity.CreatedDate = _dateTimeService.Now;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                if (entry.Entity.LastModifiedBy == Guid.Empty)
                    entry.Entity.LastModifiedBy = entry.Entity.Id;

                entry.Entity.LastModifiedDate = _dateTimeService.Now;
            }
        }
    }
}