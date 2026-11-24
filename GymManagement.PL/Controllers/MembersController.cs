using GymManagement.BLL.Common;
using GymManagement.BLL.Services.AttachmentService;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _service;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService , IAttachmentService attachmentService)
        {
            _service = memberService;
            _attachmentService = attachmentService;
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


        #region Get member photo
        [HttpGet]
        public async Task<IActionResult> Picture(int id)
        {
            var member = await _service.GetMemberDetailsByIdAsync(id);
            if ( member == null || string.IsNullOrWhiteSpace(member.Photo) )
                return NotFound();

            var Result = _attachmentService.GetFile(member.Photo , "MembersPhoto");
            if ( Result == null ) return NotFound();
            return File(Result.Value.Stream , Result.Value.ContentType);

        }
        #endregion

        #region Create Member
        // Get BaseUrl/Members/Create
        // Create - Shows member registration form
        [HttpGet]
        public IActionResult Create() => View();

        // Post BaseUrl/Members/CreateMember {Member}
        // CreateMember - Processes form submission  

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model , CancellationToken ct)
        {
            if ( !ModelState.IsValid ) return View(nameof(Create) , model);
            var Result = await _service.CreateMemberAsync(model , ct);
            if ( Result.success )
                TempData["SuccessMessage"] = "Member created successfully.";
            else
                TempData["ErrorMessage"] = Result.message;
            return RedirectToAction(nameof(Index));
        }
        #endregion
        public async Task<IActionResult> MemberDetails(int id , CancellationToken ct)
        {
            var member = await _service.GetMemberDetailsByIdAsync(id , ct);
            if ( member is null )
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        public async Task<IActionResult> HealthRecordDetails(int id , CancellationToken ct)
        {
            var healthRecord = await _service.GetMemberHealthRecordAsync(id , ct);
            if ( healthRecord is null )
            {
                TempData["ErrorMessage"] = "Health Record Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }

        #region Edit Member

        // Get BaseUrl/Members/Edit/{id}
        // Edit - Display Edit form
        [HttpGet]
        public async Task<IActionResult> EditMember(int id , CancellationToken ct)
        {
            var memberToUpdate = await _service.GetMemberToUpdateAsync(id , ct);
            if ( memberToUpdate is null )
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(memberToUpdate);
        }


        // Post BaseUrl/Members/Edit {Member}
        // Edit - Submit Form
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id , MemberToUpdateViewModel model , CancellationToken ct)
        {
            if ( !ModelState.IsValid ) return View(model);
            var Result = await _service.UpdateMemberAsync(id , model , ct);
            if ( Result )
                TempData["SuccessMessage"] = "Member Updated Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Update Member";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete Member
        // Get BaseUrl/Members/Delete/{id}
        // Delete - Show Form
        [HttpGet]
        public async Task<IActionResult> Delete(int id , CancellationToken ct)
        {
            var member = await _service.GetMemberDetailsByIdAsync(id , ct);
            if ( member is null )
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        // Post BaseUrl/members/DeleteConfirmed/{id}
        // DeleteConfirmed - Submit form 
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id , CancellationToken ct)
        {
            var Result = await _service.DeleteMemberAsync(id , ct);
            if ( Result )
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Delete Member";
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
