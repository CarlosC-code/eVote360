using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;
using eVote360.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        private readonly VoteAppContext _dbContext;
        public UsuarioRepository(VoteAppContext context) : base(context)
        {
            _dbContext = context;
        }
        public async Task<Usuario?> LoginAsync(string userName, string password)
        {

            Usuario? user = await _dbContext.Set<Usuario>().FirstOrDefaultAsync
                (u => u.UserName == userName && u.PasswordHash == password);
            return user;
        }
    }
}