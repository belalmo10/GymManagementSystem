using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.AttachmentService;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.MemberViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork , IMapper mapper , IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel member , CancellationToken ct)
        {
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(b => b.Email == member.Email);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(b => b.Phone == member.Phone);
            if ( emailExist || phoneExist ) return Common.Result.Fail("Email Or Phone Already ");

            var stordPhotoName = await _attachmentService.UploadAsync(member.PhotoFile.OpenReadStream() , member.PhotoFile.FileName , "MembersPhoto");

            if ( string.IsNullOrWhiteSpace(stordPhotoName.value) ) return Common.Result.NotFound("PhotoName Not Found");

            var memberMapped = _mapper.Map<Member>(member);
            memberMapped.Photo = stordPhotoName.value;
            _unitOfWork.GetRepository<Member>().Add(memberMapped);
            var Result = await _unitOfWork.SaveChangeAsync(ct);
            if ( Result > 0 )
            {
                return Common.Result.Ok();
            }
            else
            {
                _attachmentService.Delete(member.PhotoFile.FileName , "MembersPhoto");
                return Common.Result.Fail("Failed To Create Member");
            }
        }

        public async Task<bool> DeleteMemberAsync(int id , CancellationToken ct)
        {
            var memberToDelete = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id , ct);
            if ( memberToDelete == null ) return false;

            var hasFutureSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(s => s.Id == id && s.StartDate > DateTime.Now , ct: ct);
            if ( hasFutureSessions ) return false;

            _unitOfWork.GetRepository<Member>().Delete(memberToDelete);
            var Result = await _unitOfWork.SaveChangeAsync(ct);
            return Result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if ( !members.Any() )
                return [];

            var ViewModels = _mapper.Map<IEnumerable<Member> , IEnumerable<MemberViewModel>>(members);
            return ViewModels;
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int id , CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id , ct);
            if ( member == null )
                return null;

            var memberViewModel = _mapper.Map<Member , MemberViewModel>(member);
            var membership = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(m => m.MemberId == id && m.EndDate > DateTime.Now , ct: ct);
            if ( membership != null )
            {
                var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(membership.PlanId , ct);
                memberViewModel.PlanName = plan?.Name;
                memberViewModel.MembershipStartDate = membership.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = membership.EndDate.ToShortDateString();
            }
            return memberViewModel;
        }

        public async Task<HealthRecordViewModel> GetMemberHealthRecordAsync(int id , CancellationToken ct)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(hr => hr.MemberId == id , ct: ct);
            if ( healthRecord is null )
                return null;
            return _mapper.Map<HealthRecord , HealthRecordViewModel>(healthRecord);
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id , CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id , ct);
            if ( member == null )
                return null;

            return _mapper.Map<Member , MemberToUpdateViewModel>(member);
        }

        public async Task<bool> UpdateMemberAsync(int id , MemberToUpdateViewModel member , CancellationToken ct)
        {
            var memberToUpdate = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id , ct);
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(b => b.Email == member.Email && b.Id != id);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(b => b.Phone == member.Phone && b.Id != id);

            if ( phoneExist || emailExist ) return false;

            _mapper.Map(member , memberToUpdate);
            memberToUpdate.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(memberToUpdate);
            var Result = await _unitOfWork.SaveChangeAsync(ct);
            return Result > 0;
        }
    }
}
