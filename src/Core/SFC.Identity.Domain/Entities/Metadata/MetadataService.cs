using SFC.Identity.Domain.Common;

namespace SFC.Identity.Domain.Entities.Metadata;
public class MetadataService : EnumEntity<MetadataServiceEnum>
{
    public MetadataService() : base() { }

    public MetadataService(MetadataServiceEnum enumType) : base(enumType) { }
}