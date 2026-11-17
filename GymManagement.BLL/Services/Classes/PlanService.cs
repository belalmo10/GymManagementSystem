using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.PlanViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            return plans.Select(p => new PlanViewModel
            {
                Id = p.Id ,
                Name = p.Name ,
                Description = p.Description ,
                DurationDays = p.DurationDays ,
                Price = p.Price ,
                IsActive = p.IsActive
            });
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int id , CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id , ct: ct);
            if ( plan == null )
                return null;

            return new PlanViewModel
            {
                Name = plan.Name ,
                Price = plan.Price ,
                DurationDays = plan.DurationDays ,
                Description = plan.Description ,
                IsActive = plan.IsActive
            };
        }

        public async Task<UpdatePlanViewModel> GetPlanToUpdateAsync(int id , CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id , ct);
            if ( plan == null || !plan.IsActive == true )
                return null;

            if ( await CheckHasAnyActivateMemberShip(id , ct) )  // Focus Here
                return null;

            var plantoUpdate = new UpdatePlanViewModel
            {
                PlanName = plan.Name ,
                Price = plan.Price ,
                Description = plan.Description ,
                DurationDays = plan.DurationDays
            };

            return plantoUpdate;
        }

        public async Task<bool> TogglePlanActivationAsync(int id , CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id , ct);
            if ( plan == null )
                return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangeAsync(ct);
            return result > 0;
        }

        public async Task<bool> UpdatePlanAsync(int id , UpdatePlanViewModel model , CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id , ct);
            if ( plan == null )
                return false;

            if ( await CheckHasAnyActivateMemberShip(id , ct) )
                return false;

            plan.Price = model.Price;
            plan.Description = model.Description;
            plan.DurationDays = model.DurationDays;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangeAsync(ct);
            return result > 0;
        }

        private async Task<bool> CheckHasAnyActivateMemberShip(int id , CancellationToken ct)  // Focus Here
        {
            return await _unitOfWork.GetRepository<MemberShip>().AnyAsync(p => p.Id == id && p.EndDate > DateTime.Now , ct);
        }
    }
}
