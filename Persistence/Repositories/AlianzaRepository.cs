using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;
using eVote360.Infrastructure.Persistence.Contexts;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class AlianzaRepository : GenericRepository<Alianza>, IAlianzaRepository
    {
        public AlianzaRepository(VoteAppContext context) : base(context)
        {
        }
    }
}