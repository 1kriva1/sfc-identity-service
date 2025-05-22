using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SFC.Identity.Application.Interfaces.Persistence.Repository.Common;
using SFC.Identity.Application.Interfaces.Persistence.Repository.Metadata;
using SFC.Identity.Application.Interfaces.Persistence.Repository.User;
using SFC.Identity.Infrastructure.Persistence.Repositories.Common;
using SFC.Identity.Infrastructure.Persistence.Repositories.Metadata;
using SFC.Identity.Infrastructure.Persistence.Repositories.User;

namespace SFC.Identity.Infrastructure.Persistence.Extensions;
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IRepository<,,>), typeof(Repository<,,>));
        services.AddScoped<IMetadataRepository, MetadataRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}