using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class EleccionPuestoEntityConfiguration : IEntityTypeConfiguration<EleccionPuesto>
    {

        public void Configure(EntityTypeBuilder<EleccionPuesto> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("EleccionPuestos");
            #endregion

            #region Property configurations
            builder.Property(ep => ep.EleccionId).IsRequired();
            builder.Property(ep => ep.PuestoElectivoId).IsRequired();
            #endregion

            #region Relationships
            builder.HasOne(ep => ep.Eleccion)
                   .WithMany(e => e.EleccionPuestos)
                   .HasForeignKey(ep => ep.EleccionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ep => ep.PuestoElectivo)
                   .WithMany(p => p.EleccionPuestos)
                   .HasForeignKey(ep => ep.PuestoElectivoId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Indexes / Constraints
            builder.HasIndex(x => new { x.EleccionId, x.PuestoElectivoId }).IsUnique();
            #endregion
        }
    }

}

