using Microsoft.AspNetCore.Mvc;

using SFC.Identity.Application;
using SFC.Identity.Infrastructure;
using SFC.Identity.Infrastructure.Extensions;

namespace SFC.Identity.Api.Extensions;

public static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationServices();

        builder.Services.AddPersistenceServices(builder.Configuration);

        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services.Configure<MvcOptions>(options => options.AllowEmptyInputInBodyModelBinding = true);

        builder.Services.AddCors();

        builder.Services.AddControllers();

        builder.Services.AddLocalization();

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

        // commented for GRPC
        app.UseHttpsRedirection();

        app.UseLocalization();

        app.UseCustomExceptionHandler();

        app.UseRouting();

        app.UseEndpoints(configure => { });

        // add identity server (Duende) to pipeline
        app.UseIdentityServer();

        app.UseAuthorization();

        app.MapControllers();

        app.UseGrpc();

        app.UseSpa();

        return app;
    }
}
