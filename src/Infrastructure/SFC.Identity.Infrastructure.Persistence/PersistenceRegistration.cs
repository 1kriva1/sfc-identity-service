using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SFC.Identity.Infrastructure.Persistence;
using SFC.Identity.Infrastructure.Persistence.Seeds.Users;

namespace SFC.Identity.Infrastructure;

public static class PersistenceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("IdentityConnectionString"),
            b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));
    }

    public static async Task ResetDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        try
        {
            IdentityDbContext? context = scope.ServiceProvider.GetService<IdentityDbContext>();

            if (context != null)
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
                await context.SeedUsers();
            }
        }
        catch (Exception ex)
        {
            ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}
