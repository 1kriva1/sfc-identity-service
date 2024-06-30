using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using SFC.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using Duende.IdentityServer.EntityFramework.DbContexts;
using SFC.Identity.Application.Common.Constants;
using Environments = SFC.Identity.Application.Common.Constants.Environments;

namespace SFC.Identity.Api.IntegrationTests.Fixtures;

public class CustomWebApplicationFactory<TStartup>
       : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // remove db contexts
            RemoveServiceDescriptor<DbContextOptions<IdentityDbContext>>(services);
            RemoveServiceDescriptor<DbContextOptions<ConfigurationDbContext>>(services);
            RemoveServiceDescriptor<DbContextOptions<PersistedGrantDbContext>>(services);

            // remove db connection
            RemoveServiceDescriptor<DbConnection>(services);

            // create open SqliteConnection so EF won't automatically close it
            services.AddSingleton<DbConnection>(container =>
            {
                SqliteConnection connection = new("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            // switch db context connection to sqllite db
            services.AddDbContext<IdentityDbContext>(SwitchToSqliteConnection);
            services.AddDbContext<ConfigurationDbContext>(SwitchToSqliteConnection);
            services.AddDbContext<PersistedGrantDbContext>(SwitchToSqliteConnection);
        });

        builder.UseEnvironment(Environments.Testing);
    }

    public void InitializeDbForTests()
    {
        using IServiceScope scope = Services.CreateScope();

        IServiceProvider scopedServices = scope.ServiceProvider;

        IdentityDbContext context = scopedServices.GetRequiredService<IdentityDbContext>();

        context.Database.EnsureCreated();

        Utilities.InitializeDbForTests(context);
    }

    private static void RemoveServiceDescriptor<T>(IServiceCollection services)
    {
        ServiceDescriptor? serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        services.Remove(serviceDescriptor!);
    }

    private static void SwitchToSqliteConnection(IServiceProvider container, DbContextOptionsBuilder options)
    {
        DbConnection connection = container.GetRequiredService<DbConnection>();
        options.UseSqlite(connection);
    }
}
