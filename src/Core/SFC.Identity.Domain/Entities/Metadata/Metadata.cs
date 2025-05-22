namespace SFC.Identity.Domain.Entities.Metadata;
public class Metadata
{
    public required MetadataServiceEnum Service { get; set; }

    public required MetadataTypeEnum Type { get; set; }

    public required MetadataStateEnum State { get; set; }

    public required MetadataDomainEnum Domain { get; set; }
}