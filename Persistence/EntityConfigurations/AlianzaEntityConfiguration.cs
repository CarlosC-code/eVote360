using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class AlianzaEntityConfiguration : IEntityTypeConfiguration<Alianza>
    {
        
        public void Configure(EntityTypeBuilder<Alianza> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Alianzas", t =>
            {
                
                t.HasCheckConstraint("CK_Alianza_A_B_Distintos", "[PartidoAId] <> [PartidoBId]");
            });

            #endregion

            #region Property configurations
            builder.Property(a => a.PartidoAId).IsRequired();
            builder.Property(a => a.PartidoBId).IsRequired();
            builder.Property(a => a.FechaAceptacionUtc).IsRequired();
            #endregion

            #region Relationships
            builder.HasOne(a => a.PartidoA)
                   .WithMany(p => p.AlianzasA)
                   .HasForeignKey(a => a.PartidoAId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.PartidoB)
                   .WithMany(p => p.AlianzasB)
                   .HasForeignKey(a => a.PartidoBId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion

        }
    }
}
