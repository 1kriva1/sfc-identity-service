using Microsoft.AspNetCore.SpaServices.AngularCli;

using SFC.Identity.Api.Infrastructure.Extensions;

namespace SFC.Identity.Api.Infrastructure.Extensions;

public static class SpaExtensions
{
    private const string SOURCE_PATH = "../../Client/SFC.Identity.Client";
    private const string ROOT_PATH = "/dist/sfc-identity";
    private const int STARTUP_TIMEOUT = 300;

    public static void AddSpa(this IServiceCollection services)
    {
        services.AddSpaStaticFiles(configuration => configuration.RootPath = $"{SOURCE_PATH}{ROOT_PATH}");
    }

    public static void UseSpa(this WebApplication app)
    {
        app.UseSpa(spa =>
        {
            if (app.Environment.IsDevelopment())
            {
                spa.Options.SourcePath = SOURCE_PATH;
                spa.Options.StartupTimeout = TimeSpan.FromSeconds(STARTUP_TIMEOUT);
                spa.UseAngularCliServer(npmScript: "start");
            }
        });
    }
}