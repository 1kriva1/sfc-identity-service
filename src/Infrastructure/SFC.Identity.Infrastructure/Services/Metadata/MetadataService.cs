using SFC.Identity.Application.Interfaces.Metadata;
using SFC.Identity.Application.Interfaces.Persistence.Repository.Metadata;
using SFC.Identity.Domain.Enums.Metadata;

namespace SFC.Identity.Infrastructure.Services.Metadata;
public class MetadataService(IMetadataRepository metadataRepository) : IMetadataService
{
    private readonly IMetadataRepository _metadataRepository = metadataRepository;

    public async Task CompleteAsync(MetadataServiceEnum service, MetadataDomainEnum domain, MetadataTypeEnum type)
    {
        MetadataEntity? metadata = await _metadataRepository.GetByIdAsync(service, domain, type).ConfigureAwait(true);

        if (metadata is not null)
        {
            metadata.State = MetadataStateEnum.Done;
            await _metadataRepository.UpdateAsync(metadata).ConfigureAwait(false);
        }
    }

    public async Task<bool> IsCompletedAsync(MetadataServiceEnum service, MetadataDomainEnum domain, MetadataTypeEnum type)
    {
        MetadataEntity? metadata = await _metadataRepository.GetByIdAsync(service, domain, type).ConfigureAwait(true);
        return metadata?.State == MetadataStateEnum.Done;
    }
}