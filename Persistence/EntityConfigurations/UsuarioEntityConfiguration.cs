using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class UsuarioEntityConfiguration : IEntityTypeConfiguration<Usuario>
    {

        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Usuarios");
            #endregion

            #region Property configurations
            builder.Property(u => u.Nombre).IsRequired();
            builder.Property(u => u.Apellido).IsRequired();
            builder.Property(u => u.Email).IsRequired();
            builder.Property(u => u.UserName).IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired(); 
            builder.Property(u => u.Rol).IsRequired();
            builder.HasIndex(u => u.UserName).IsUnique();
            #endregion

            #region Relationships
            builder.HasMany(u => u.DirigentePartidos)
                   .WithOne(dp => dp.Usuario)
                   .HasForeignKey(dp => dp.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }

}
