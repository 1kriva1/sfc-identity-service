using Duende.IdentityModel;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SFC.Identity.Infrastructure.Configuration;
using SFC.Identity.Infrastructure.Persistence.Constants;
using SFC.Identity.Infrastructure.Persistence.Contexts;
using SFC.Identity.Infrastructure.Persistence.Entities;
using SFC.Identity.Infrastructure.Services.Identity;
using SFC.Identity.Infrastructure.Settings;
using SFC.Identity.Infrastructure.Validators;

using ApiResourceEntity = Duende.IdentityServer.EntityFramework.Entities.ApiResource;
using ApiScopeEntity = Duende.IdentityServer.EntityFramework.Entities.ApiScope;
using ClientEntity = Duende.IdentityServer.EntityFramework.Entities.Client;
using IdentityConstants = SFC.Identity.Infrastructure.Constants.IdentityConstants;

namespace SFC.Identity.Infrastructure.Extensions;
public static class IdentityExtensions
{
    public static void AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")!;
        string migrationsAssemblyName = typeof(IdentityDbContext).Assembly.FullName!;

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

        services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            options.UserInteraction.LoginUrl = "/identity/login";
            options.UserInteraction.LogoutUrl = "/identity/logout";
            options.UserInteraction.CreateAccountUrl = "/identity/registration";
            options.UserInteraction.ErrorUrl = "/error";

            options.UserInteraction.LoginReturnUrlParameter = "returnUrl";
            options.UserInteraction.CreateAccountReturnUrlParameter = "returnUrl";
            options.UserInteraction.LogoutIdParameter = "logoutId";

            // add audience claim to token
            options.EmitStaticAudienceClaim = true;
        })
        // this adds the config data from DB (clients, resources, CORS)
        .AddConfigurationStore(options =>
        {
            options.ConfigureDbContext = builder =>
                builder.UseSqlServer(connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssemblyName));
            options.DefaultSchema = DatabaseConstants.DefaultSchemaName;
        })
        // this adds the operational data from DB (codes, tokens, consents)
        .AddOperationalStore(options =>
        {
            options.ConfigureDbContext = builder =>
                builder.UseSqlServer(connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssemblyName));
            options.DefaultSchema = DatabaseConstants.DefaultSchemaName;

            // this enables automatic token cleanup. this is optional.
            options.EnableTokenCleanup = true;
            options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
        })
        .AddAspNetIdentity<ApplicationUser>()
        // registers extension grant validator for the token exchange grant type
        .AddExtensionGrantValidator<TokenExchangeGrantValidator>()
        // register a profile service to emit the act claim
        .AddProfileService<ProfileService>();
    }

    public static async Task EnsureIdentityConfigurationExistAsync(this ConfigurationDbContext context, IdentitySettings settings, CancellationToken cancellationToken)
    {
        List<Task> tasks = [];

        // what was returned as a claims in token
        if (!context.IdentityResources.Any())
        {
            tasks.Add(context.IdentityResources.AddRangeAsync(IdentityConfiguration.IdentityResources.Select(resource => resource.ToEntity()), cancellationToken));
        }

        // the list of APIs (like resource)
        if (settings.Api.Resources.Count != 0 && !context.ApiResources.Any())
        {
            IEnumerable<ApiResourceEntity> resources = settings.Api.Resources.Select(resource => new ApiResource
            {
                Name = resource.Name,
                DisplayName = resource.DisplayName,
                Scopes = resource.Scopes,
                UserClaims = resource.UserClaims
            }.ToEntity());

            tasks.Add(context.ApiResources.AddRangeAsync(resources, cancellationToken));
        }

        // more granually autherization (read or write scope or full scope(read and write) or other permissions)
        if (settings.Api.Scopes.Count != 0 && !context.ApiScopes.Any())
        {
            IEnumerable<ApiScopeEntity> scopes = settings.Api.Scopes.Select(scope => new ApiScope
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName
            }.ToEntity());

            tasks.Add(context.ApiScopes.AddRangeAsync(scopes, cancellationToken));
        }

        if (settings.Clients.Count != 0 && !context.Clients.Any())
        {
            IEnumerable<ClientEntity> clients = settings.Clients.Select(client => new Client
            {
                ClientId = client.Id,
                ClientName = client.Name,
                ClientSecrets = client.Secrets.Select(secret => new Secret(secret.Sha256()))
                                              .ToArray(),
                // authorization code flow
                AllowedGrantTypes = client.IsTokenExchange
                    ? [OidcConstants.GrantTypes.TokenExchange]
                    : GrantTypes.Code,
                AllowOfflineAccess = client.AllowOfflineAccess,
                RedirectUris = client.RedirectUris,
                PostLogoutRedirectUris = client.PostLogoutRedirectUris,
                AllowedScopes = client.Scopes,
                // skip consent screen
                RequireConsent = false,
                UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
                IdentityTokenLifetime = client.IdentityTokenLifetime ?? IdentityConstants.DefaultIdentityTokenLifetime,
                AccessTokenLifetime = client.AccessTokenLifetime ?? IdentityConstants.DefaultAccessTokenLifetime,
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime ?? IdentityConstants.DefaultAbsoluteRefreshTokenLifetime,
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime ?? IdentityConstants.DefaultSlidingRefreshTokenLifetime,
            }.ToEntity());

            tasks.Add(context.Clients.AddRangeAsync(clients, cancellationToken));
        }

        if (tasks.Count != 0)
        {
            await Task.WhenAll(tasks).ConfigureAwait(true);

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(true);
        }
    }
}