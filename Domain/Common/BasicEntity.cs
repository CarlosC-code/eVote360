
namespace eVote360.Core.Domain.Common
{
    public class BasicEntity<Tkey>
    {
        public required Tkey Id { get; set; }                 
        public bool IsActive { get; set; } = true;  


    }
}
