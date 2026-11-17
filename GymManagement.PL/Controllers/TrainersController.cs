using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.TrainerViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var trainers = await _trainerService.GetAllTrainerAsync(ct);
            return View(trainers);
        }

        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsByIdAsync(id , ct);
            if ( trainer is null )
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        #region Create
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model , CancellationToken ct)
        {
            if ( !ModelState.IsValid ) return View(model);

            var Result = await _trainerService.CreateTrainerAsync(model , ct);
            if ( Result )
                TempData["SuccessMessage"] = "Trainer Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Trainer";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainertoUpdateAsync(id , ct);
            if ( trainer is null )
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id , TrainerToUpdateViewModel model , CancellationToken ct)
        {
            if ( !ModelState.IsValid ) return View(model);

            var Result = await _trainerService.UpdateTrainerAsync(id , model , ct);
            if ( Result )
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Update Trainer";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id , CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsByIdAsync(id , ct);
            if ( trainer is null )
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id , CancellationToken ct)
        {
            var trainer = await _trainerService.DeleteTrainerAsync(id , ct);
            if ( trainer )
                TempData["SuccessMessage"] = "Trainer Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Delete Trainer";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
