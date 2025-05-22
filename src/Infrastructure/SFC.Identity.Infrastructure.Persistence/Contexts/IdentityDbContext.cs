using System.Reflection;
using System.Reflection.Emit;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using SFC.Identity.Application.Interfaces.Persistence.Context;
using SFC.Identity.Domain.Entities.User;
using SFC.Identity.Infrastructure.Persistence.Constants;
using SFC.Identity.Infrastructure.Persistence.Entities;
using SFC.Identity.Infrastructure.Persistence.Interceptors;

namespace SFC.Identity.Infrastructure.Persistence.Contexts;

public class IdentityDbContext(
    IHostEnvironment hostEnvironment,
    DbContextOptions<IdentityDbContext> options,
    AuditableUserEntitySaveChangesInterceptor auditableInterceptor) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), IIdentityDbContext
{
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
    private readonly AuditableUserEntitySaveChangesInterceptor _auditableInterceptor = auditableInterceptor;

    public IQueryable<IUser> IdentityUsers => Set<IUser>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // important to call this first, because table names will NOT override
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(DatabaseConstants.DefaultSchemaName);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        MetadataDbContext.ApplyMetadataConfigurations(builder, _hostEnvironment.IsDevelopment());
    }
}