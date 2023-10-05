using Microsoft.AspNetCore.Mvc;

using SFC.Identity.Api.Filters;
using SFC.Identity.Api.Middlewares;
using SFC.Identity.Application;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Infrastructure;

namespace SFC.Identity.Api;

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

        builder.AddLocalization();

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

        // commented for GRPC
        //app.UseHttpsRedirection();

        app.UseLocalization();

        app.UseAuthorization();

        app.UseCustomExceptionHandler();

        app.MapControllers();

        app.UseGrpc();

        return app;
    }

    private static void AddLocalization(this WebApplicationBuilder builder)
    {
        builder.Services.AddLocalization(options => options.ResourcesPath = CommonConstants.RESOURCE_PATH);

        builder.Services.Configure<RequestLocalizationOptions>(options => options.SetDefaultCulture(CommonConstants.SUPPORTED_CULTURES[0])
                .AddSupportedCultures(CommonConstants.SUPPORTED_CULTURES)
                .AddSupportedUICultures(CommonConstants.SUPPORTED_CULTURES));
    }
}
