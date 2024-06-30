using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SFC.Data.Infrastructure.Services.Hosted;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Infrastructure.Extensions;
using SFC.Identity.Infrastructure.Services;
using SFC.Identity.Infrastructure.Settings;

namespace SFC.Identity.Infrastructure;

public static class InfrastructureRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // identity
        services.AddIdentity(configuration);

        // grpc
        services.AddGrpc();

        // settings
        services.Configure<IdentitySettings>(configuration.GetSection(IdentitySettings.SECTION_KEY));

        // services
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IExistenceService, ExistenceService>();

        // hosted services
        services.AddHostedService<DatabaseResetHostedService>();
    }
}
