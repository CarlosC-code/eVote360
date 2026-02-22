using eVote360.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace eVote360.Infrastructure.Persistence.Contexts
{
    public class VoteAppContext : DbContext
    {
        public VoteAppContext(DbContextOptions<VoteAppContext> options) : base(options)  { }

        public DbSet<Alianza> Alianzas { get; set; }
        public DbSet<AlianzaSolicitud> Solicitudes { get; set; }
        public DbSet<Candidato> Candidatos { get; set; }
        public DbSet<Candidatura> Candidaturas { get; set; }
        public DbSet<Ciudadano> Ciudadanos { get; set; }
        public DbSet<DirigentePartido> Dirigentes { get; set; }
        public DbSet<Eleccion> Elecciones { get; set; }
        public DbSet<EleccionCandidatura> EleccionCandidaturas { get; set; }
        public DbSet<EleccionPuesto> EleccionPuestos { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<PuestoElectivo> PuestoElectivos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Voto> Votos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //Liskov-substitution

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
