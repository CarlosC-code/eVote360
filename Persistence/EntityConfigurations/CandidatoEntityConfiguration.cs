using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class CandidatoEntityConfiguration : IEntityTypeConfiguration<Candidato>
    {

        public void Configure(EntityTypeBuilder<Candidato> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Candidatos");
            #endregion

            #region Property configurations
            builder.Property(c => c.Nombre).IsRequired();
            builder.Property(c => c.Apellido).IsRequired();
            builder.Property(c => c.PartidoId).IsRequired();
            #endregion

            #region Relationships
            builder.HasOne(c => c.Partido)
                   .WithMany(p => p.Candidatos)
                   .HasForeignKey(c => c.PartidoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.PuestoElectivoOrigen)
                   .WithMany(p => p.CandidatosOrigen)
                   .HasForeignKey(c => c.PuestoElectivoOrigenId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Candidaturas)
                   .WithOne(ca => ca.Candidato)
                   .HasForeignKey(ca => ca.CandidatoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Votos)
                   .WithOne(v => v.Candidato)
                   .HasForeignKey(v => v.CandidatoId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }

}

