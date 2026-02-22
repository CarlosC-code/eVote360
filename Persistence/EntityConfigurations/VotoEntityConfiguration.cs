using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class VotoEntityConfiguration : IEntityTypeConfiguration<Voto>
    {
        public void Configure(EntityTypeBuilder<Voto> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Votos");
            #endregion

            #region Property configurations
            builder.Property(v => v.EleccionId).IsRequired();
            builder.Property(v => v.CiudadanoId).IsRequired();
            builder.Property(v => v.PuestoElectivoId).IsRequired();
            builder.Property(v => v.EsNinguno).IsRequired();
            builder.Property(v => v.FechaEmisionUtc).IsRequired();
            #endregion

            #region Relationships
            builder.HasOne(v => v.Eleccion)
                   .WithMany(e => e.Votos)
                   .HasForeignKey(v => v.EleccionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.Ciudadano)
                   .WithMany(c => c.Votos)
                   .HasForeignKey(v => v.CiudadanoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.PuestoElectivo)
                   .WithMany(p => p.Votos)
                   .HasForeignKey(v => v.PuestoElectivoId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Nulo cuando el voto es "Ninguno"
            builder.HasOne(v => v.Candidatura)
                   .WithMany(ca => ca.Votos)
                   .HasForeignKey(v => v.CandidaturaId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }

}

