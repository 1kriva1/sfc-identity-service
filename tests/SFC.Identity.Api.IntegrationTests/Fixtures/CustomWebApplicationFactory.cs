using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using SFC.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace SFC.Identity.Api.IntegrationTests.Fixtures
{
    public class CustomWebApplicationFactory<TStartup>
           : WebApplicationFactory<TStartup> where TStartup : class
    {
        private const string TEST_ENVIROMENT = "Testing";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<IdentityDbContext>));

                services.Remove(dbContextDescriptor!);

                ServiceDescriptor? dbConnectionDescriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbConnection));

                services.Remove(dbConnectionDescriptor!);

                // Create open SqliteConnection so EF won't automatically close it.
                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<IdentityDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });
            });

            builder.UseEnvironment(TEST_ENVIROMENT);
        }

        public void InitializeDbForTests()
        {
            using IServiceScope scope = Services.CreateScope();

            IServiceProvider scopedServices = scope.ServiceProvider;

            IdentityDbContext context = scopedServices.GetRequiredService<IdentityDbContext>();

            context.Database.EnsureCreated();

            Utilities.InitializeDbForTests(context);
        }
    }
}
