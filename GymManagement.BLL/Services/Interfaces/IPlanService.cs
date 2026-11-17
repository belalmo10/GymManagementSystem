using GymManagement.BLL.Services.ViewModels.PlanViewModel;
using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct);
        Task<PlanViewModel?> GetPlanByIdAsync(int id, CancellationToken ct);
        Task<UpdatePlanViewModel> GetPlanToUpdateAsync(int id, CancellationToken ct);
        Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct);
        Task<bool> TogglePlanActivationAsync(int id, CancellationToken ct);
    }
}
