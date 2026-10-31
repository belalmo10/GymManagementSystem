using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.MemberViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GymManagement.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _service;
        public MembersController(IMemberService memberService )
        {
            _service = memberService;
        }

        // Get BaseUrl/Members/Index
        // Index - List All Members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Members = await _service.GetAllMemberAsync(ct);
            return View(Members); 
        }


        // Get BaseUrl/Members/MemberDetails/{id}
        // MemberDetails - Show one member's details

        // Get BaseUrl/Members/HealthRecordDetails/{id}
        // HealthRecordDetails - Shows health record page 


        #region Create Member
        // Get BaseUrl/Members/Create
        // Create - Shows member registration form
        [HttpGet]
        public IActionResult Create() => View();

        // Post BaseUrl/Members/CreateMember {Member}
        // CreateMember - Processes form submission  

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create),model);
            var Result = await _service.CreateMemberAsync(model, ct);
            if (Result)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Member";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit Member

        // Get BaseUrl/Members/Edit/{id}
        // Edit - Display Edit form

        // Post BaseUrl/Members/Edit {Member}
        // Edit - Submit Form

        #endregion

        #region Delete Member
        // Get BaseUrl/Members/Delete/{id}
        // Delete - Show Form

        // Post BaseUrl/members/DeleteConfirmed/{id}
        // DeleteConfirmed - Submit form 
        #endregion
    }
}
