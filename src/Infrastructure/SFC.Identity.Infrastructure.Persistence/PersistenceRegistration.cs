using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SFC.Identity.Infrastructure.Persistence;

namespace SFC.Identity.Infrastructure;

public static class PersistenceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("IdentityConnectionString"),
            b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));
    }
}
