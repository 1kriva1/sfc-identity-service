using Microsoft.EntityFrameworkCore;

using SFC.Identity.Infrastructure.Persistence.Interceptors;

namespace SFC.Identity.Infrastructure.Persistence.Contexts;
public abstract class BaseDbContext<T>(
    DbContextOptions<T> options,
    DispatchDomainEventsSaveChangesInterceptor eventsInterceptor) : DbContext(options) where T : DbContext
{
    private readonly DispatchDomainEventsSaveChangesInterceptor _eventsInterceptor = eventsInterceptor;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        optionsBuilder.AddInterceptors(_eventsInterceptor);
    }
}