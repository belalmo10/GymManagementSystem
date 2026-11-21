using GymManagement.BLL.Common;
using GymManagement.BLL.Services.ViewModels.SessionViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default);
        Task<Result> CreateSessionAsync(CreateSessionViewModel model , CancellationToken ct = default);

        Task<IEnumerable<CategorySelectedViewModel>> GetCategoryFromDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<TrainerSelectedViewModel>> GetTrainerFromDropDownAsync(CancellationToken ct = default);
        Task<Result<SessionViewModel>> GetSessionDetailsAsync(int id , CancellationToken ct = default);
        Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id , CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int id , UpdateSessionViewModel model , CancellationToken ct = default);
        Task<Result> DeleteSessionAsync(int id , CancellationToken ct = default);
    }
}
