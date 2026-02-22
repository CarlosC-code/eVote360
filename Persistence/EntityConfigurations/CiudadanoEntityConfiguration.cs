using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace eVote360.Infrastructure.Persistence.EntityConfigurations
{
    public class CiudadanoEntityConfiguration : IEntityTypeConfiguration<Ciudadano>
    {
        public void Configure(EntityTypeBuilder<Ciudadano> builder)
        {
            // Fluent API
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Ciudadanos");
            #endregion

            #region Property configurations
            builder.Property(u => u.Nombre).IsRequired();
            builder.Property(u => u.Apellido).IsRequired();
            builder.Property(u => u.Email).IsRequired();
            builder.Property(u => u.DocumentoIdentidad).IsRequired();
            builder.HasIndex(u => u.DocumentoIdentidad).IsUnique();
            #endregion

            #region Relationships
            builder.HasMany(c => c.Votos)
                   .WithOne(v => v.Ciudadano)
                   .HasForeignKey(v => v.CiudadanoId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }

}

