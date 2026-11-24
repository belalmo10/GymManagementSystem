using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAnalyticsService _service;

        public HomeController(IAnalyticsService Service)
        {
            _service = Service;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            return View(await _service.GetAllAnalyticsAsync(ct));
        }


    }
}
