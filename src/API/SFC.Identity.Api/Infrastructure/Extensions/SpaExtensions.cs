using Microsoft.AspNetCore.SpaServices.AngularCli;

using SFC.Identity.Api.Infrastructure.Extensions;
using SFC.Identity.Infrastructure.Extensions;

namespace SFC.Identity.Api.Infrastructure.Extensions;

public static class SpaExtensions
{
    private const string SOURCE_PATH = "../../Client/SFC.Identity.Client";
    private const string ROOT_PATH = "wwwroot";
    private const int STARTUP_TIMEOUT = 300;

    public static void AddSpa(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            builder.Services.AddSpaStaticFiles(configuration => configuration.RootPath = ROOT_PATH);
        }
    }

    public static void UseSpa(this WebApplication app)
    {
        app.UseSpa(spa =>
        {
            if (app.Environment.IsDevelopment())
            {
                string npmScript = EnvironmentExtensions.IsRunningInContainer
                    ? "start:container:development" : "start:development";

                spa.Options.SourcePath = SOURCE_PATH;
                spa.Options.StartupTimeout = TimeSpan.FromSeconds(STARTUP_TIMEOUT);
                spa.UseAngularCliServer(npmScript);
            }
        });
    }
}