using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.PlanViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Controllers
{
    public class PlansController : Controller
    {

        private readonly IPlanService _planService;
        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var plans = await _planService.GetAllPlansAsync(ct);
            return View(plans);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id , CancellationToken ct = default)
        {
            var plan = await _planService.GetPlanByIdAsync(id , ct);
            if ( plan == null )
            {
                TempData["ErrorMessage"] = "Plan Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct = default)
        {
            var plan = await _planService.GetPlanToUpdateAsync(id , ct);
            if ( plan == null )
            {
                TempData["ErrorMessage"] = "Plan Can Not Be Edited Cause May (Not Found , Has Active MemberShip , InActive)";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id , UpdatePlanViewModel model , CancellationToken ct = default)
        {
            if ( !ModelState.IsValid )
                return View(model);
            var plan = await _planService.UpdatePlanAsync(id , model , ct);
            if ( plan )
                TempData["SuccessMessage"] = "Plan Updated Successfully";
            else
                TempData["ErrorMessage"] = "Plan Failed To Update";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id , CancellationToken ct = default)
        {
            var plan = await _planService.TogglePlanActivationAsync(id , ct);
            if ( plan )
                TempData["SuccessMessage"] = "Plan Status Changed";
            else
                TempData["ErrorMessage"] = "Failed To Update Plan Status";
            return RedirectToAction(nameof(Index));
        }
    }
}
