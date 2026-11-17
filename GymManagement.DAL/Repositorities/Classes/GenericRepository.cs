using GymManagement.DAL.Data;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Intefaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GymManagement.DAL.Repositorities.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext dbContext;
        private readonly DbSet<TEntity> _set;
        public GenericRepository(GymDbContext context)
        {
            dbContext = context;
            _set = dbContext.Set<TEntity>();
        }
        public void Add(TEntity entity)
        {
            _set.Add(entity);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity , bool>> predicate , CancellationToken ct = default)
        {
            return _set.AsNoTracking().AnyAsync(predicate , ct);
        }

        public void Delete(TEntity entity)
        {
            _set.Remove(entity);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity , bool>> predicate , bool tracking = false , CancellationToken ct = default)
        {
            var result = tracking ? _set.FirstOrDefaultAsync(predicate , ct) : _set.AsNoTracking().FirstOrDefaultAsync(predicate , ct);
            return await result;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false , CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id , CancellationToken ct = default)
        {
            return await _set.FindAsync(id , ct);
        }

        public void Update(TEntity entity)
        {
            _set.Update(entity);
        }
    }
}
