using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFC.Identity.Api.Filters;
using SFC.Identity.Api.Middlewares;
using SFC.Identity.Application;
using SFC.Identity.Application.Common.Constants;
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

            builder.Services.AddLocalization(options => options.ResourcesPath = CommonConstants.RESOURCE_PATH);

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture(CommonConstants.SUPPORTED_CULTURES[0])
                    .AddSupportedCultures(CommonConstants.SUPPORTED_CULTURES)
                    .AddSupportedUICultures(CommonConstants.SUPPORTED_CULTURES);
            });

            builder.Services.AddCors();

            builder.Services.AddControllers(config =>
            {
                config.Filters.Add(new ValidationFilterAttribute());
            })
            .AddJsonOptions(configure =>
            {
                configure.JsonSerializerOptions.PropertyNamingPolicy = null;
                configure.JsonSerializerOptions.DictionaryKeyPolicy = null;
                configure.JsonSerializerOptions.WriteIndented = true;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            })
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(Resources));
            });

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseHttpsRedirection();

            app.UseLocalization();

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
