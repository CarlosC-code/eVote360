
using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Partido : BasicEntity<int>
    {
        public required string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }
        public required string Siglas { get; set; } = default!;
        public string? LogoPath { get; set; }

        // Navegacion
        public ICollection<Candidato>? Candidatos { get; set; } 
        public ICollection<DirigentePartido>? DirigentePartidos { get; set; } 
        public ICollection<Candidatura>? Candidaturas { get; set; } 

        // Alianzas (pares A-B)
        public ICollection<Alianza>? AlianzasA { get; set; } 
        public ICollection<Alianza>? AlianzasB { get; set; }

        // Solicitudes de alianza
        public ICollection<AlianzaSolicitud>? SolicitudesEnviadas { get; set; } 
        public ICollection<AlianzaSolicitud>? SolicitudesRecibidas { get; set; } 

    }
}
