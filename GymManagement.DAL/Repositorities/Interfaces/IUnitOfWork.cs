using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Classes;

namespace GymManagement.DAL.Repositorities.Interfaces
{
    public interface IUnitOfWork
    {
        GenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();
        Task<int> SaveChangeAsync(CancellationToken ct);
        ISessionRepository SessionRepository { get; }
    }
}
