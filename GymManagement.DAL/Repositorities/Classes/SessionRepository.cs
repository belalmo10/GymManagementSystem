using GymManagement.DAL.Data;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositorities.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _context;

        public SessionRepository(GymDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct = default)
        {
            var session = _context.Sessions.AsNoTracking().Include(s => s.Trainer).Include(x => x.Category);
            return await session.ToListAsync();
        }

        public async Task<int> GetCountOfSlotsAsync(int sessionId , CancellationToken ct = default)
        {
            return await _context.Bookings.AsNoTracking().CountAsync(s => s.SessionId == sessionId);
        }

        public async Task<Session?> GetSessionWithTrainerAndCategory(int sessionId , CancellationToken ct = default)
        {
            return await _context.Sessions.AsNoTracking()
                                          .Include(s => s.Trainer)
                                          .Include(x => x.Category).FirstOrDefaultAsync(s => s.Id == sessionId);
        }
    }
}
