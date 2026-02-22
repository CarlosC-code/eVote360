
using System.Linq.Expressions;
using eVote360.Core.Domain.Interfaces;
using eVote360.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        protected readonly VoteAppContext _context;
        protected readonly DbSet<TEntity> _set;

        public GenericRepository(VoteAppContext context)
        {
            _context = context;
            _set = _context.Set<TEntity>();
        }

        // LECTURA
        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _set.FindAsync(new object?[] { id }, ct);

        public async Task<List<TEntity>> GetAllListAsync(CancellationToken ct = default)
            => await _set.AsNoTracking().ToListAsync(ct);

        public async Task<List<TEntity>> GetAllListWithIncludeAsync(
            CancellationToken ct = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _set.AsNoTracking();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.ToListAsync(ct);
        }

        public IQueryable<TEntity> GetAllQuery()
            => _set.AsQueryable();

        public IQueryable<TEntity> GetAllQueryWithInclude(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _set.AsQueryable();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _set.AnyAsync(predicate, ct);

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(predicate).ToListAsync(ct);

        // ESCRITURA (con commit y retorno)
        public async Task<TEntity?> AddAsync(TEntity entity, CancellationToken ct = default)
        {
            await _set.AddAsync(entity, ct);
            var rows = await _context.SaveChangesAsync(ct);
            return rows > 0 ? entity : null;
        }

        public async Task<List<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
        {
            var list = entities.ToList();
            if (list.Count == 0) return list;
            await _set.AddRangeAsync(list, ct);
            await _context.SaveChangesAsync(ct);
            return list;
        }

        public async Task<TEntity?> UpdateAsync(int id, TEntity newValues, CancellationToken ct = default)
        {
            var current = await _set.FindAsync(new object?[] { id }, ct);
            if (current is null) return null;

            _context.Entry(current).CurrentValues.SetValues(newValues);
            var rows = await _context.SaveChangesAsync(ct);
            return rows > 0 ? current : null;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _set.FindAsync(new object?[] { id }, ct);
            if (entity is null) return false;

            _set.Remove(entity);
            var rows = await _context.SaveChangesAsync(ct);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(TEntity entity, CancellationToken ct = default)
        {
            _set.Remove(entity);
            var rows = await _context.SaveChangesAsync(ct);
            return rows > 0;
        }
    }
}
