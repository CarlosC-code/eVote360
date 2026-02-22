using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class CandidaturaEntityConfiguration : IEntityTypeConfiguration<Candidatura>
    {

        public void Configure(EntityTypeBuilder<Candidatura> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Candidaturas");
            #endregion

            #region Property configurations
            builder.Property(ca => ca.CandidatoId).IsRequired();
            builder.Property(ca => ca.PartidoId).IsRequired();
            builder.Property(ca => ca.PuestoElectivoId).IsRequired();
            #endregion

            #region Relationships
            builder.HasOne(ca => ca.Candidato)
                   .WithMany(c => c.Candidaturas)
                   .HasForeignKey(ca => ca.CandidatoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ca => ca.Partido)
                   .WithMany(p => p.Candidaturas)
                   .HasForeignKey(ca => ca.PartidoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ca => ca.PuestoElectivo)
                   .WithMany(p => p.Candidaturas)
                   .HasForeignKey(ca => ca.PuestoElectivoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ca => ca.EleccionCandidaturas)
                   .WithOne(ec => ec.Candidatura)
                   .HasForeignKey(ec => ec.CandidaturaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ca => ca.Votos)
                   .WithOne(v => v.Candidatura)
                   .HasForeignKey(v => v.CandidaturaId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Indexes / Constraints
            // 1) un partido NO puede tener 2 candidatos para el mismo puesto
            builder.HasIndex(x => new { x.PartidoId, x.PuestoElectivoId }).IsUnique();

            // 2) un candidato NO puede tener 2 candidaturas diferentes dentro del mismo partido
            builder.HasIndex(x => new { x.PartidoId, x.CandidatoId }).IsUnique();
            #endregion
        }
    }

}

