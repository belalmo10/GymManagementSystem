using GymManagement.DAL.Data;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.DAL.Repositorities.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _context;
        private readonly Dictionary<string , object> _repositories = [];
        public UnitOfWork(GymDbContext context , ISessionRepository sessionRepository)
        {
            _context = context;
            SessionRepository = sessionRepository;
        }

        public ISessionRepository SessionRepository { get; }

        public GenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;

            if ( _repositories.TryGetValue(typeName , out object? value) )
                return (GenericRepository<TEntity>) value;

            else
            {
                var repo = new GenericRepository<TEntity>(_context);
                _repositories[typeName] = repo;
                return repo;
            }
        }

        public async Task<int> SaveChangeAsync(CancellationToken ct) => await _context.SaveChangesAsync(ct);

    }
}
