using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SFC.Identity.Infrastructure.Persistence.Extensions;

namespace SFC.Identity.Infrastructure.Persistence.Extensions;
public static class ContextExtensions
{
    public static void AddDbContext<T>(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment environment) where T : DbContext
    {
        bool isDevelopment = environment.IsDevelopment();

        services.AddDbContext<T>(options => options
            .UseSqlServer(configuration.GetConnectionString("Database"), b =>
            {
                b.MigrationsAssembly(typeof(T).Assembly.FullName);
                b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            })
            .EnableDetailedErrors(isDevelopment)
            .EnableSensitiveDataLogging(isDevelopment)
        );
    }
}