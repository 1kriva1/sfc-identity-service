using Microsoft.AspNetCore.Builder;

using SFC.Identity.Infrastructure.Services.Internal;

namespace SFC.Identity.Infrastructure.Extensions;
public static class GrpcExtensions
{
    public static WebApplication UseGrpc(this WebApplication app)
    {
        app.MapGrpcService<IdentityService>();
        return app;
    }
}
