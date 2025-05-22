using Google.Api;

using SFC.Identity.Application;
using SFC.Identity.Infrastructure;
using SFC.Identity.Infrastructure.Extensions;
using SFC.Identity.Infrastructure.Persistence;

namespace SFC.Identity.Api.Infrastructure.Extensions;

public static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationServices();

        builder.AddPersistenceServices();

        builder.AddInfrastructureServices();

        builder.Services.AddApiServices();

        builder.Services.AddControllers();

        builder.Services.AddLocalization();

        builder.AddAuthentication();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSwagger();
        }

        if (builder.Environment.IsProduction())
        {
            builder.Services.AddSpa();
        }

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

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
        }

        if (app.Environment.IsProduction())
        {
            app.UseSpaStaticFiles();
        }

        app.UseHttpsRedirection();

        app.UseLocalization();

        app.UseCustomExceptionHandler();

        app.UseRouting();

        app.UseLocalization();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(configure => { });

        // add identity server (Duende) to pipeline
        app.UseIdentityServer();

        app.MapControllers();

        app.UseGrpc();

        app.UseSpa();

        return app;
    }
}