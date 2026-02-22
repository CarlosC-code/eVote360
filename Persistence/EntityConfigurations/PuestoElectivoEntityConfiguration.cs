using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class PuestoElectivoEntityConfiguration : IEntityTypeConfiguration<PuestoElectivo>
    {

        public void Configure(EntityTypeBuilder<PuestoElectivo> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("PuestosElectivos");
            #endregion

            #region Property configurations
            builder.Property(p => p.Nombre).IsRequired();
            builder.Property(p => p.Descripcion).IsRequired();
            #endregion

            #region Relationships
            builder.HasMany(p => p.CandidatosOrigen)
                   .WithOne(c => c.PuestoElectivoOrigen)
                   .HasForeignKey(c => c.PuestoElectivoOrigenId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Candidaturas)
                   .WithOne(ca => ca.PuestoElectivo)
                   .HasForeignKey(ca => ca.PuestoElectivoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.EleccionPuestos)
                   .WithOne(ep => ep.PuestoElectivo)
                   .HasForeignKey(ep => ep.PuestoElectivoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Votos)
                   .WithOne(v => v.PuestoElectivo)
                   .HasForeignKey(v => v.PuestoElectivoId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
