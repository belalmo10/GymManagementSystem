using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Intefaces;

namespace GymManagement.DAL.Repositorities.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct = default);
        Task<int> GetCountOfSlotsAsync(int sessionId , CancellationToken ct = default);
        Task<Session?> GetSessionWithTrainerAndCategory(int sessionId , CancellationToken ct = default);

    }
}
