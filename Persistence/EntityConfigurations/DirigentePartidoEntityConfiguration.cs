using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class DirigentePartidoEntityConfiguration : IEntityTypeConfiguration<DirigentePartido>
    {

        public void Configure(EntityTypeBuilder<DirigentePartido> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("DirigentesPartidos");
            #endregion

            #region Property configurations
            builder.Property(dp => dp.UsuarioId).IsRequired();
            builder.Property(dp => dp.PartidoId).IsRequired();
            #endregion

            #region Relationships
            builder.HasOne(dp => dp.Usuario)
                   .WithMany(u => u.DirigentePartidos)
                   .HasForeignKey(dp => dp.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dp => dp.Partido)
                   .WithMany(p => p.DirigentePartidos)
                   .HasForeignKey(dp => dp.PartidoId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Indexes
            builder.HasIndex(dp => dp.UsuarioId);
            #endregion
        }
    }

}
