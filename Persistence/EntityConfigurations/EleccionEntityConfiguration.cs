using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class EleccionEntityConfiguration : IEntityTypeConfiguration<Eleccion>
    {

        public void Configure(EntityTypeBuilder<Eleccion> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Elecciones");
            #endregion

            #region Property configurations
            builder.Property(e => e.Nombre).IsRequired();
            builder.Property(e => e.FechaRealizacion).IsRequired();
            builder.Property(e => e.Estado).IsRequired();
            #endregion

            #region Relationships
            builder.HasMany(e => e.EleccionPuestos)
                   .WithOne(ep => ep.Eleccion)
                   .HasForeignKey(ep => ep.EleccionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.EleccionCandidaturas)
                   .WithOne(ec => ec.Eleccion)
                   .HasForeignKey(ec => ec.EleccionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Votos)
                   .WithOne(v => v.Eleccion)
                   .HasForeignKey(v => v.EleccionId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }

}

