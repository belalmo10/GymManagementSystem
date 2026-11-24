using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.AnalyticsViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AnalyticsService(IUnitOfWork UnitOfWork)
        {
            _unitOfWork = UnitOfWork;
        }
        public async Task<AnalyticsViewModel> GetAllAnalyticsAsync(CancellationToken ct)
        {
            var totalMembers = await _unitOfWork.GetRepository<Member>().CountAsync(ct: ct);
            var totalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct);
            var activeMembers = await _unitOfWork.GetRepository<MemberShip>().CountAsync(m => m.EndDate > DateTime.Now , ct);
            var upcomingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate > DateTime.Now);
            var ongoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(X => X.StartDate <= DateTime.Now && X.EndDate >= DateTime.Now);
            var completedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(X => X.EndDate < DateTime.Now);

            return new AnalyticsViewModel()
            {
                TotalMembers = totalMembers ,
                TotalTrainers = totalTrainers ,
                ActiveMembers = activeMembers ,
                UpcomingSessions = upcomingSessions ,
                OngoingSessions = ongoingSessions ,
                CompletedSessions = completedSessions
            };
        }
    }
}
