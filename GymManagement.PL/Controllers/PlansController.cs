using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.PlanViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Controllers
{
    [Authorize]
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
            if ( plan.success )
                return View(plan.value);
            else
            {
                TempData["ErrorMessage"] = plan.message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct = default)
        {
            var plan = await _planService.GetPlanToUpdateAsync(id , ct);
            if ( plan.success )
                return View(plan.value);
            else
            {
                TempData["ErrorMessage"] = plan.message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id , UpdatePlanViewModel model , CancellationToken ct = default)
        {
            if ( !ModelState.IsValid )
                return View(model);
            var plan = await _planService.UpdatePlanAsync(id , model , ct);
            if ( plan.success )
                TempData["SuccessMessage"] = "Plan Updated Successfully";
            else
                TempData["ErrorMessage"] = "Plan Failed To Update";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id , CancellationToken ct = default)
        {
            var plan = await _planService.TogglePlanActivationAsync(id , ct);
            if ( plan.success )
                TempData["SuccessMessage"] = "Plan Status Changed";
            else
                TempData["ErrorMessage"] = plan.message;
            return RedirectToAction(nameof(Index));
        }
    }
}
