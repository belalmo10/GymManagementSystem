using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Classes;
using GymManagement.DAL.Repositorities.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Controllers
{
    public class PlansController : Controller
    {
        //private readonly GymDbContext dbContext;
        //public PlansController()
        //{
        //    dbContext = new GymDbContext();
        //}

        private readonly IGenericRepository<Plan> planRepository;
        public PlansController(IGenericRepository<Plan> repository)
        {
            planRepository = repository;
        }
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var plans = await planRepository.GetAllAsync(ct: ct);
            return View(plans);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var plan = await planRepository.GetByIdAsync(id, ct);
            if(plan == null)
                return RedirectToAction(nameof(Index));
            return View(plan);
        }
    }
}
