using AutoMapper;
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

        public MemberService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel member , CancellationToken ct)
        {
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(b => b.Email == member.Email);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(b => b.Phone == member.Phone);
            if ( emailExist || phoneExist ) return false;

            var memberMapped = _mapper.Map<Member>(member);

            _unitOfWork.GetRepository<Member>().Add(memberMapped);
            var Result = await _unitOfWork.SaveChangeAsync(ct);
            return Result > 0;
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
