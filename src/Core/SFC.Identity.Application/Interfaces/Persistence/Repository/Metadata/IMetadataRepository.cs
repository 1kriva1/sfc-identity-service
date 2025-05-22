using SFC.Identity.Application.Interfaces.Persistence.Context;
using SFC.Identity.Application.Interfaces.Persistence.Repository.Common;

namespace SFC.Identity.Application.Interfaces.Persistence.Repository.Metadata;
public interface IMetadataRepository : IRepository<MetadataEntity, IMetadataDbContext, int>
{
    Task<MetadataEntity?> GetByIdAsync(MetadataServiceEnum service, MetadataDomainEnum domain, MetadataTypeEnum type);
}