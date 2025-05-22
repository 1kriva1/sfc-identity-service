using Google.Api;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using SFC.Identity.Api.Infrastructure.Authentication;
using SFC.Identity.Api.Infrastructure.Extensions;
using SFC.Identity.Infrastructure.Extensions;
using SFC.Identity.Infrastructure.Settings;

namespace SFC.Identity.Api.Infrastructure.Extensions;

public static class AuthenticationExtensions
{
    private const string VALID_JWT_HEADER_TYP = "at+jwt";

    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        IdentitySettings identitySettings = builder.Configuration.GetIdentitySettings();

        builder.Services.AddSingleton<AuthenticationJwtBearerEvents>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
             {
                 if (!builder.Environment.IsDevelopment() || builder.Configuration.UseAuthentication())
                 {
                     options.Authority = identitySettings.Authentication.Authority;
                     options.Audience = identitySettings.Authentication.Audience;
                     options.TokenValidationParameters = new()
                     {
                         ValidateAudience = true,
                         NameClaimType = "name",
                         ValidTypes = [VALID_JWT_HEADER_TYP]
                     };
                 }

                 options.EventsType = typeof(AuthenticationJwtBearerEvents);
             }
         );

        builder.Services.AddAuthorization(options =>
        {
            options.AddGeneralPolicy(identitySettings.Authentication.RequireClaims);
        });
    }
}