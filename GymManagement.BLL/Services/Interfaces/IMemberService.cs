using GymManagement.BLL.Services.ViewModels.MemberViewModel;
using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default);
        Task<bool> CreateMemberAsync(CreateMemberViewModel member, CancellationToken ct);
        Task<MemberViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken ct);
        Task<HealthRecordViewModel> GetMemberHealthRecordAsync(int id, CancellationToken ct);
        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id, CancellationToken ct);
        Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel member, CancellationToken ct);
        Task<bool> DeleteMemberAsync(int id, CancellationToken ct);

    }
}
