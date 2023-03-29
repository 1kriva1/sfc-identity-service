using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFC.Identity.Api.Filters;
using SFC.Identity.Api.Middlewares;
using SFC.Identity.Application;
using SFC.Identity.Infrastructure;
using SFC.Identity.Infrastructure.Persistence;

namespace SFC.Identity.Api
{
    public static class StartupExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddApplicationServices();

            builder.Services.AddPersistenceServices(builder.Configuration);

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.Configure<MvcOptions>(options =>
            {
                options.AllowEmptyInputInBodyModelBinding = true;
            });

            builder.Services.AddControllers(config =>
            {
                config.Filters.Add(new ValidationFilterAttribute());
            })
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
                o.JsonSerializerOptions.WriteIndented = true;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return builder.Build();

        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCustomExceptionHandler();

            app.MapControllers();

            return app;

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
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
    }
}
