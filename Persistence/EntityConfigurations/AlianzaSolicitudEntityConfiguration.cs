using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class AlianzaSolicitudEntityConfiguration : IEntityTypeConfiguration<AlianzaSolicitud>
    {
        
        public void Configure(EntityTypeBuilder<AlianzaSolicitud> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("AlianzasSolicitudes", t =>
            {
                t.HasCheckConstraint("CK_AlianzaSolicitud_Solicitante_Destino",
                                     "[PartidoSolicitanteId] <> [PartidoDestinoId]");
            });

            #endregion

            #region Property configurations
            builder.Property(a => a.PartidoSolicitanteId).IsRequired();
            builder.Property(a => a.PartidoDestinoId).IsRequired();
            builder.Property(a => a.Estado).IsRequired();
            builder.Property(a => a.FechaSolicitudUtc).IsRequired();
            #endregion

            #region Relationships
            builder.HasOne(a => a.PartidoSolicitante)
                   .WithMany(p => p.SolicitudesEnviadas)
                   .HasForeignKey(a => a.PartidoSolicitanteId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.PartidoDestino)
                   .WithMany(p => p.SolicitudesRecibidas)
                   .HasForeignKey(a => a.PartidoDestinoId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}