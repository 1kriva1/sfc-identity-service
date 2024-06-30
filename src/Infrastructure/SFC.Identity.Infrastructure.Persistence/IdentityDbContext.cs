using System.Reflection;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Infrastructure.Persistence.Models;

namespace SFC.Identity.Infrastructure.Persistence;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(DbConstants.DEFAULT_SCHEMA_NAME);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
