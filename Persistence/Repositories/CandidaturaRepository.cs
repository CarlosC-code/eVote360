using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;
using eVote360.Infrastructure.Persistence.Contexts;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class CandidaturaRepository : GenericRepository<Candidatura>, ICandidaturaRepository
    {
        public CandidaturaRepository(VoteAppContext context) : base(context)
        {
        }
    }
}