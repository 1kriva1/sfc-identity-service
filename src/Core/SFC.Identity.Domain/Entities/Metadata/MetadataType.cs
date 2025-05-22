using SFC.Identity.Domain.Common;

namespace SFC.Identity.Domain.Entities.Metadata;
public class MetadataType : EnumEntity<MetadataTypeEnum>
{
    public MetadataType() : base() { }

    public MetadataType(MetadataTypeEnum enumType) : base(enumType) { }
}