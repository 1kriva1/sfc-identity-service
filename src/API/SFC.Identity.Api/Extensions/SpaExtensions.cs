using Microsoft.AspNetCore.SpaServices.AngularCli;

namespace SFC.Identity.Api.Extensions;

public static class SpaExtensions
{
    private const string SOURCE_PATH = "../../Client/SFC.Identity.Client";
    private const string ROOT_PATH = "/dist/sfc-identity";

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
                spa.UseAngularCliServer(npmScript: "start");
            }
        });
    }
}
