using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SFC.Identity.Application.Models.Tokens;
using SFC.Identity.Infrastructure.Persistence;
using SFC.Identity.Infrastructure.Persistence.Models;

namespace SFC.Identity.Api.IntegrationTests.Fixtures
{
    public class Utilities
    {
        public static Guid USER_ID = Guid.Parse("{6313179F-7837-473A-A4D5-A5571B43E6A6}");

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

            EntityEntry<AccessToken> accessEntry = context.AccessTokens.Add(new AccessToken
            {
                CreatedDate = DateTime.Now,
                ExpiresDate = DateTime.MaxValue,
                UserForeignKey = USER_ID,
                Value = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSW50ZWdyYXRpb25UZXN0VXNlciIsImV4cCI6MTY4MjQwNTkxOCwiaXNzIjoiR2xvYm9UaWNrZXRJZGVudGl0eSIsImF1ZCI6Ikdsb2JvVGlja2V0SWRlbnRpdHlVc2VyIn0.sniULKxCa7wBI3OGqJMayaBdKo7zwPdo91XCt7cyleo"
            });

            context.RefreshTokens.Add(new RefreshToken
            {
                TokenForeignKey = accessEntry.Entity.Id,
                CreatedDate = DateTime.Now,
                ExpiresDate = DateTime.MaxValue,
                Value = "kaGWK0agkRpoKjvRvnV+R/jswQJ1aqeszK9V61xN9vj0lD+yBj9EXFeM1GmWjChmtVeXWppJptak1uOOdpkBkA=="
            });

            context.SaveChanges();
        }
    }
}
