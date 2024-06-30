using Microsoft.AspNetCore.Identity;

using SFC.Identity.Infrastructure.Persistence;
using SFC.Identity.Infrastructure.Persistence.Models;

namespace SFC.Identity.Api.IntegrationTests.Fixtures;

public class Utilities
{
    private static readonly Guid USER_ID = Guid.Parse("{6313179F-7837-473A-A4D5-A5571B43E6A6}");

    public static void InitializeDbForTests(IdentityDbContext context)
    {
        if (context.Users.Any(user => user.Id == USER_ID))
            return;

        context.Users.Add(new ApplicationUser
        {
            Id = USER_ID,
            UserName = "IntegrationTestUser",
            NormalizedUserName = "INTEGRATIONTESTUSER",
            Email = "integrationtestemail@mail.com",
            NormalizedEmail = "INTEGRATIONTESTEMAIL@MAIL.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            LockoutEnabled = true,
            PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null!, "Test1234!")
        });

        context.SaveChanges();
    }
}
