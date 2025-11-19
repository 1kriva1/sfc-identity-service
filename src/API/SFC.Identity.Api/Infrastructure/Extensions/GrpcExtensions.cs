using SFC.Identity.Api.Services;

namespace SFC.Identity.Api.Infrastructure.Extensions;

public static class GrpcExtensions
{
    public static WebApplication UseGrpc(this WebApplication app)
    {
        app.MapGrpcService<IdentityService>();
        return app;
    }
}