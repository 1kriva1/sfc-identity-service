using System.Reflection;

using Microsoft.AspNetCore.Mvc;

using SFC.Identity.Api.Extensions;
using SFC.Identity.Api.Filters;
using SFC.Identity.Application;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Infrastructure;

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

        // commented for GRPC
        //app.UseHttpsRedirection();

        app.UseLocalization();

        app.UseAuthorization();

        app.UseCustomExceptionHandler();

        app.MapControllers();

        app.UseGrpc();

        return app;
    }
}
