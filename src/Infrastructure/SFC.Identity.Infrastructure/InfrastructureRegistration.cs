using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SFC.Data.Infrastructure.Services.Hosted;
using SFC.Identity.Application.Interfaces;
using SFC.Identity.Infrastructure.Persistence;
using SFC.Identity.Infrastructure.Persistence.Models;
using SFC.Identity.Infrastructure.Services;
using SFC.Identity.Infrastructure.Settings;

using InfrastructureIdentityService = SFC.Identity.Infrastructure.Services.IdentityService;

namespace SFC.Identity.Infrastructure;

public static class InfrastructureRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
          .AddEntityFrameworkStores<IdentityDbContext>()
          .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = false;
        });

        services.AddGrpc();        

        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.AddTransient<IJwtService, JwtService>();
        services.AddTransient<IIdentityService, InfrastructureIdentityService>();
        services.AddTransient<IExistenceService, ExistenceService>();

        services.AddHostedService<DatabaseResetHostedService>();
    }

    public static WebApplication UseGrpc(this WebApplication app)
    {
        app.MapGrpcService<InternalIdentityService>();

        return app;
    }
}
