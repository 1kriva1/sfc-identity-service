using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SFC.Identity.Api.Middlewares.Exception;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application;

namespace SFC.Identity.Api.Middlewares;

public static class MiddlewareExtensions
{
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

    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
