
using System.Linq.Expressions;

namespace eVote360.Core.Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        // LECTURA
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<TEntity>> GetAllListAsync(CancellationToken ct = default);
        Task<List<TEntity>> GetAllListWithIncludeAsync(
            CancellationToken ct = default,
            params Expression<Func<TEntity, object>>[] includes);

        IQueryable<TEntity> GetAllQuery();
        IQueryable<TEntity> GetAllQueryWithInclude(params Expression<Func<TEntity, object>>[] includes);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);

        // ESCRITURA (con commit interno y retorno)
        Task<TEntity?> AddAsync(TEntity entity, CancellationToken ct = default);
        Task<List<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
        Task<TEntity?> UpdateAsync(int id, TEntity newValues, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> DeleteAsync(TEntity entity, CancellationToken ct = default);
    }
}

