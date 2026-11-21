using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.SessionViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model , CancellationToken ct)
        {
            if ( !ModelState.IsValid )
            {
                await PopulateDropDownListAsync();
                return View(model);
            }
            var session = await _sessionService.CreateSessionAsync(model , ct);
            if ( session.success )
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = session.message;
            await PopulateDropDownListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var session = await _sessionService.GetSessionDetailsAsync(id);
            if ( session.success )
                return View(session.value);
            else
            {
                TempData["ErrorMessage"] = session.message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var session = await _sessionService.GetSessionToUpdateAsync(id , ct);
            if ( session.success )
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerFromDropDownAsync() , "Id" , "Name");
                return View(session.value);
            }
            else
            {
                TempData["ErrorMessage"] = session.message;
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id , UpdateSessionViewModel model , CancellationToken ct)
        {
            if ( !ModelState.IsValid )
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerFromDropDownAsync() , "Id" , "Name");
                return View(model);
            }

            var result = await _sessionService.UpdateSessionAsync(id , model , ct);
            if ( result.success )
            {
                TempData["SuccessMessage"] = "Session Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.message;
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerFromDropDownAsync() , "Id" , "Name");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id , CancellationToken ct)
        {
            var session = await _sessionService.GetSessionDetailsAsync(id , ct);
            if ( session.success )
            {
                return View(session.value);
            }
            else
            {
                ViewBag["ErrorMessage"] = session.message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id , CancellationToken ct)
        {
            var session = await _sessionService.DeleteSessionAsync(id , ct);

            TempData[session.success ? "SuccessMessage" : "ErrorMessage"] = session.success ? "Session Deleted Successfully" : session.message;
            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateDropDownListAsync()
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerFromDropDownAsync() , "Id" , "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetCategoryFromDropDownAsync() , "Id" , "CategoryName");
        }
    }

}
