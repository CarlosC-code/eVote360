namespace eVote360.Core.Application.ViewModels.Votacion
{
    public class CandidaturaOptionViewModel
    {
        public int? CandidaturaId { get; set; }      
        public string Texto { get; set; } = default!; 
        public string? FotoPath { get; set; }
        public bool Selected { get; set; }
    }
}
