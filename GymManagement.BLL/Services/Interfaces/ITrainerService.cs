using GymManagement.BLL.Services.ViewModels.TrainerViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainerAsync(CancellationToken ct);
        Task<TrainerDetailsViewModel?> GetTrainerDetailsByIdAsync(int id , CancellationToken ct);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel model , CancellationToken ct);
        Task<TrainerToUpdateViewModel?> GetTrainertoUpdateAsync(int id , CancellationToken ct);
        Task<bool> UpdateTrainerAsync(int id , TrainerToUpdateViewModel model , CancellationToken ct);
        Task<bool> DeleteTrainerAsync(int id , CancellationToken ct);

    }
}
