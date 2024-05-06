using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using SFC.Identity.Application;
using SFC.Identity.Application.Common.Constants;

namespace SFC.Identity.Api.Extensions;

public static class LocalizationExtensions
{
    public static void AddLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = CommonConstants.RESOURCE_PATH);
        services.Configure<RequestLocalizationOptions>(options => options.SetDefaultCulture(CommonConstants.SUPPORTED_CULTURES[0])
                .AddSupportedCultures(CommonConstants.SUPPORTED_CULTURES)
                .AddSupportedUICultures(CommonConstants.SUPPORTED_CULTURES));
    }

    public static void UseLocalization(this WebApplication app)
    {
        IOptions<RequestLocalizationOptions> localizationOptions =
            app.Services.GetService<IOptions<RequestLocalizationOptions>>()!;

        app.UseRequestLocalization(localizationOptions.Value);

        // inject localizer explicity for error messages
        IStringLocalizer<Resources> localizer =
            app.Services.GetService<IStringLocalizer<Resources>>()!;

        Messages.Configure(localizer);
    }
}
