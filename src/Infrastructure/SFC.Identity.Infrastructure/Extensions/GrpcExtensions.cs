using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SFC.Identity.Infrastructure.Interceptors.Grpc.Server;
using SFC.Identity.Infrastructure.Settings;

namespace SFC.Identity.Infrastructure.Extensions;
public static class GrpcExtensions
{
    public static void AddGrpc(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        GrpcSettings settings = configuration.GetGrpcSettings();

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = environment.IsDevelopment();
            options.MaxReceiveMessageSize = settings.MaxReceiveMessageSizeInMb.ToByteSize();
            options.MaxSendMessageSize = settings.MaxSendMessageSizeInMb.ToByteSize();

            // interceptors
            options.Interceptors.Add<ServerExceptionInterceptor>();
            options.Interceptors.Add<ServerLoggingInterceptor>();
            options.Interceptors.Add<ServerLanguageInterceptor>();
        });
    }
}