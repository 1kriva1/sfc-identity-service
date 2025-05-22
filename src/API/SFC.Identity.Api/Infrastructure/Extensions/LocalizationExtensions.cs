using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using SFC.Identity.Api.Infrastructure.Extensions;
using SFC.Identity.Application;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Infrastructure.Constants;

namespace SFC.Identity.Api.Infrastructure.Extensions;

public static class LocalizationExtensions
{
    public static void AddLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = CommonConstants.ResourcePath);
        services.Configure<RequestLocalizationOptions>(options => options.SetDefaultCulture(CommonConstants.SupportedCultures[0])
                .AddSupportedCultures(CommonConstants.SupportedCultures)
                .AddSupportedUICultures(CommonConstants.SupportedCultures));
    }

    public static void UseLocalization(this WebApplication app)
    {
        IOptions<RequestLocalizationOptions> localizationOptions =
            app.Services.GetService<IOptions<RequestLocalizationOptions>>()!;

        app.UseRequestLocalization(localizationOptions.Value);

        // inject localizer explicity for error messages
        IStringLocalizer<Resources> localizer =
            app.Services.GetService<IStringLocalizer<Resources>>()!;

        Localization.Configure(localizer);
    }
}