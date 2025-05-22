namespace SFC.Identity.Application.Interfaces.Persistence.Context;
public interface IMetadataDbContext : IDbContext
{
    IQueryable<MetadataEntity> Metadata { get; }
}