using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using SFC.Identity.Application.Interfaces.Common;
using SFC.Identity.Application.Interfaces.Existence;
using SFC.Identity.Application.Interfaces.Identity;
using SFC.Identity.Application.Interfaces.Metadata;
using SFC.Identity.Infrastructure.Extensions;
using SFC.Identity.Infrastructure.Services.Common;
using SFC.Identity.Infrastructure.Services.Hosted;
using SFC.Identity.Infrastructure.Services.Identity;
using SFC.Identity.Infrastructure.Services.Metadata;
using SFC.Identity.Infrastructure.Settings;

namespace SFC.Identity.Infrastructure;

public static class InfrastructureRegistration
{
    public static void AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // identity
        builder.Services.AddIdentity(builder.Configuration);

        // rabbitmq
        builder.AddMassTransit();

        // grpc
        builder.Services.AddGrpc(builder.Configuration, builder.Environment);

        // settings
        builder.Services.Configure<IdentitySettings>(builder.Configuration.GetSection(IdentitySettings.SectionKey));

        // services
        builder.Services.AddTransient<IMetadataService, MetadataService>();
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddTransient<IExistenceService, ExistenceService>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IDateTimeService, DateTimeService>();
        builder.Services.AddTransient<IUserSeedService, UserSeedService>();

        // hosted services
        builder.Services.AddHostedService<DatabaseResetHostedService>();
        builder.Services.AddHostedService<DataInitializationHostedService>();
    }
}