using SFC.Identity.Domain.Enums.Metadata;

namespace SFC.Identity.Application.Interfaces.Metadata;
public interface IMetadataService
{
    Task CompleteAsync(MetadataServiceEnum service, MetadataDomainEnum domain, MetadataTypeEnum type);

    Task<bool> IsCompletedAsync(MetadataServiceEnum service, MetadataDomainEnum domain, MetadataTypeEnum type);
}