using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SFC.Identity.Application.Models.Tokens;

namespace SFC.Identity.Infrastructure.Persistence.Configurations;
public class AccessTokenConfiguration : IEntityTypeConfiguration<AccessToken>
{
    public void Configure(EntityTypeBuilder<AccessToken> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .ValueGeneratedOnAdd()
               .IsRequired(true);

        builder.HasOne(b => b.RefreshToken)
               .WithOne(i => i.Token)
               .HasForeignKey<RefreshToken>(b => b.TokenForeignKey)
               .IsRequired(true);

        builder.Navigation(e => e.RefreshToken)
               .AutoInclude(true);
    }
}
