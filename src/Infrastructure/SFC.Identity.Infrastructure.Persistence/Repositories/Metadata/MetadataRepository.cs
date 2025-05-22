using Microsoft.EntityFrameworkCore;

using SFC.Identity.Application.Interfaces.Persistence.Repository.Metadata;
using SFC.Identity.Infrastructure.Persistence.Contexts;
using SFC.Identity.Infrastructure.Persistence.Repositories.Common;

namespace SFC.Identity.Infrastructure.Persistence.Repositories.Metadata;
public class MetadataRepository(MetadataDbContext context)
    : Repository<MetadataEntity, MetadataDbContext, int>(context), IMetadataRepository
{
    public Task<MetadataEntity?> GetByIdAsync(MetadataServiceEnum service, MetadataDomainEnum domain, MetadataTypeEnum type)
    {
        return Context.Metadata.FirstOrDefaultAsync(metadata =>
            metadata.Service == service &&
            metadata.Domain == domain &&
            metadata.Type == type);
    }
}