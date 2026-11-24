using GymManagement.BLL.Services.ViewModels.AnalyticsViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetAllAnalyticsAsync(CancellationToken ct = default);
    }
}
