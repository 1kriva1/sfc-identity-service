using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Application.Models.Tokens;

namespace SFC.Identity.Infrastructure.Persistence.Configurations;
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
             .ValueGeneratedOnAdd()
             .IsRequired(true);
    }
}
