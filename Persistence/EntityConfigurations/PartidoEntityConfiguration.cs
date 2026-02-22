using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class PartidoEntityConfiguration : IEntityTypeConfiguration<Partido>
    {

        public void Configure(EntityTypeBuilder<Partido> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Partidos");
            #endregion

            #region Property configurations
            builder.Property(p => p.Nombre).IsRequired();
            builder.Property(p => p.Siglas).IsRequired();
            builder.HasIndex(p => p.Siglas).IsUnique();
            #endregion

            #region Relationships
            builder.HasMany(p => p.Candidatos)
                   .WithOne(c => c.Partido)
                   .HasForeignKey(c => c.PartidoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Candidaturas)
                   .WithOne(ca => ca.Partido)
                   .HasForeignKey(ca => ca.PartidoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.DirigentePartidos)
                   .WithOne(dp => dp.Partido)
                   .HasForeignKey(dp => dp.PartidoId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Solicitudes de alianza (enviadas/recibidas)
            builder.HasMany(p => p.SolicitudesEnviadas)
                   .WithOne(s => s.PartidoSolicitante)
                   .HasForeignKey(s => s.PartidoSolicitanteId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.SolicitudesRecibidas)
                   .WithOne(s => s.PartidoDestino)
                   .HasForeignKey(s => s.PartidoDestinoId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Alianzas (A/B)
            builder.HasMany(p => p.AlianzasA)
                   .WithOne(a => a.PartidoA)
                   .HasForeignKey(a => a.PartidoAId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.AlianzasB)
                   .WithOne(a => a.PartidoB)
                   .HasForeignKey(a => a.PartidoBId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }

}

