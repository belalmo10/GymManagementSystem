using GymManagement.BLL.Common;
using GymManagement.BLL.Services.ViewModels.PlanViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct);
        Task<Result<PlanViewModel>> GetPlanByIdAsync(int id , CancellationToken ct);
        Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int id , CancellationToken ct);
        Task<Result> UpdatePlanAsync(int id , UpdatePlanViewModel model , CancellationToken ct);
        Task<Result> TogglePlanActivationAsync(int id , CancellationToken ct);
    }
}
