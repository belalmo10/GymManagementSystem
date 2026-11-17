using GymManagement.BLL.Services.ViewModels.SessionViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default);

    }
}
