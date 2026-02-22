using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Domain.Interfaces
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
       Task<Usuario?> LoginAsync(string userName, string password);
    }
}