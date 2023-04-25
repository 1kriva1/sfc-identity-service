using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using SFC.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace SFC.Identity.Api.IntegrationTests.Fixtures
{
    public class CustomWebApplicationFactory<TStartup>
           : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<IdentityDbContext>));

                if(dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                services.AddDbContext<IdentityDbContext>((container, options) =>
                {
                    options.UseInMemoryDatabase("IdentityDbContextInMemoryTest");
                });

                ServiceProvider sp = services.BuildServiceProvider();

                using IServiceScope scope = sp.CreateScope();

                IServiceProvider scopedServices = scope.ServiceProvider;

                IdentityDbContext context = scopedServices.GetRequiredService<IdentityDbContext>();

                ILogger logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                context.Database.EnsureDeleted();

                context.Database.EnsureCreated();

                try
                {
                    Utilities.InitializeDbForTests(context);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                }
            });

            builder.UseEnvironment("Testing");
        }
    }
}
