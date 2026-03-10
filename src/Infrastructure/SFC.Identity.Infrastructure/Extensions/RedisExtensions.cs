using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SFC.Identity.Infrastructure.Settings;

using StackExchange.Redis;

namespace SFC.Identity.Infrastructure.Extensions;
public static class RedisExtensions
{
    private static readonly object Lock = new();
    private static ConnectionMultiplexer? s_mux;

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        RedisSettings settings = configuration.GetRedisSettings();

        return services.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = $"{settings.InstanceName}:";
            options.ConfigurationOptions = new ConfigurationOptions
            {
                EndPoints = { configuration.GetConnectionString("Redis")! },
                User = settings.User,
                Password = settings.Password
            };
        });
    }

    public static IDatabase GetRedisDatabase(this IConfiguration configuration, RedisSettings settings)
    {
        if (s_mux is { IsConnected: true }) return s_mux.GetDatabase();

        lock (Lock)
        {
            if (s_mux is null || !s_mux.IsConnected)
            {
                s_mux = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    EndPoints = { configuration.GetConnectionString("Redis")! },
                    User = settings.User,
                    Password = settings.Password
                });
            }

            return s_mux.GetDatabase();
        }
    }
}