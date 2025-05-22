using Microsoft.Extensions.Configuration;

using SFC.Identity.Infrastructure.Constants;
using SFC.Identity.Infrastructure.Settings;
using SFC.Identity.Infrastructure.Settings.RabbitMq;

namespace SFC.Identity.Infrastructure.Extensions;
public static class SettingsExtensions
{
    public static bool UseAuthentication(this ConfigurationManager configuration)
        => configuration.GetValue<bool>(SettingConstants.Authentication, true);

    public static RabbitMqSettings GetRabbitMqSettings(this IConfiguration configuration)
        => configuration.GetSection(RabbitMqSettings.SectionKey)
                        .Get<RabbitMqSettings>()!;

    public static GrpcSettings GetGrpcSettings(this IConfiguration configuration)
        => configuration.GetSection(GrpcSettings.SectionKey)
                        .Get<GrpcSettings>()!;

    public static KestrelSettings GetKestrelSettings(this IConfiguration configuration)
        => configuration.GetSection(KestrelSettings.SectionKey)
                        .Get<KestrelSettings>()!;

    public static IdentitySettings GetIdentitySettings(this IConfiguration configuration)
        => configuration.GetSection(IdentitySettings.SectionKey)
                        .Get<IdentitySettings>()!;
}