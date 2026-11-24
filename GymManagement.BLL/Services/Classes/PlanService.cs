using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.PlanViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);

            var viewPlans = _mapper.Map<IEnumerable<Plan> , IEnumerable<PlanViewModel>>(plans);
            return viewPlans;
        }

        public async Task<Result<PlanViewModel>> GetPlanByIdAsync(int id , CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id , ct: ct);
            if ( plan == null )
                return Result<PlanViewModel>.NotFound("Plan Not Found");

            var mappedPlan = _mapper.Map<Plan , PlanViewModel>(plan);
            return Result<PlanViewModel>.Ok(mappedPlan);
        }

        public async Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int id , CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id , ct);
            if ( plan == null || !plan.IsActive == true )
                return Result<UpdatePlanViewModel>.NotFound("Plan Not Found");

            if ( await CheckHasAnyActivateMemberShip(id , ct) )  // Focus Here
                return Result<UpdatePlanViewModel>.Fail("Can Not Update Plan That Has Active MemberShips");

            var plantoUpdate = _mapper.Map<Plan , UpdatePlanViewModel>(plan);

            return Result<UpdatePlanViewModel>.Ok(plantoUpdate);
        }

        public async Task<Result> TogglePlanActivationAsync(int id , CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id , ct);
            if ( plan == null )
                return Result.NotFound("Plan not Found");

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangeAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Activate Or Decativate This Plan");
        }

        public async Task<Result> UpdatePlanAsync(int id , UpdatePlanViewModel model , CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id , ct);
            if ( plan == null )
                return Result.NotFound("Plan not Found");

            if ( await CheckHasAnyActivateMemberShip(id , ct) )
                return Result.Fail("Can Not Update Plan That Has Active MemberShips");

            var plantoUpdate = _mapper.Map(model , plan);
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangeAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Update This Plan");
        }

        private async Task<bool> CheckHasAnyActivateMemberShip(int id , CancellationToken ct)  // Focus Here
        {
            return await _unitOfWork.GetRepository<MemberShip>().AnyAsync(p => p.Id == id && p.EndDate > DateTime.Now , ct);
        }
    }
}
