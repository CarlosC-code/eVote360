using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class EleccionCandidaturaEntityConfiguration : IEntityTypeConfiguration<EleccionCandidatura>
    {

        public void Configure(EntityTypeBuilder<EleccionCandidatura> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("EleccionCandidaturas");
            #endregion

            #region Property configurations
            builder.Property(ec => ec.EleccionId).IsRequired();
            builder.Property(ec => ec.CandidaturaId).IsRequired();
            #endregion

            #region Relationships
            builder.HasOne(ec => ec.Eleccion)
                   .WithMany(e => e.EleccionCandidaturas)
                   .HasForeignKey(ec => ec.EleccionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ec => ec.Candidatura)
                   .WithMany(ca => ca.EleccionCandidaturas)
                   .HasForeignKey(ec => ec.CandidaturaId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Indexes / Constraints
            builder.HasIndex(x => new { x.EleccionId, x.CandidaturaId }).IsUnique();
            #endregion
        }
    }

}

