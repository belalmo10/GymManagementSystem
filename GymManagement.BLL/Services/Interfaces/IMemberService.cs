using GymManagement.BLL.Common;
using GymManagement.BLL.Services.ViewModels.MemberViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default);
        Task<Result> CreateMemberAsync(CreateMemberViewModel member , CancellationToken ct);
        Task<MemberViewModel?> GetMemberDetailsByIdAsync(int id , CancellationToken ct = default);
        Task<HealthRecordViewModel> GetMemberHealthRecordAsync(int id , CancellationToken ct);
        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id , CancellationToken ct);
        Task<bool> UpdateMemberAsync(int id , MemberToUpdateViewModel member , CancellationToken ct);
        Task<bool> DeleteMemberAsync(int id , CancellationToken ct);

    }
}
