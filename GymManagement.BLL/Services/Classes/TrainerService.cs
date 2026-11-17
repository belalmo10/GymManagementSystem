using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.TrainerViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainerAsync(CancellationToken ct)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            if ( !trainers.Any() )
                return [];
            var trainerModel = trainers.Select(t => new TrainerViewModel()
            {
                Id = t.Id ,
                Name = t.Name ,
                Email = t.Email ,
                Phone = t.Phone ,
                Specialization = t.Speciality
            });

            return trainerModel;
        }

        public async Task<TrainerDetailsViewModel?> GetTrainerDetailsByIdAsync(int id , CancellationToken ct)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id , ct: ct);
            if ( trainer == null )
                return null;

            var TrainerViewModel = new TrainerDetailsViewModel()
            {
                Name = trainer.Name ,
                Email = trainer.Email ,
                Phone = trainer.Phone ,
                DateOfBirth = trainer.DateOfBirth ,
                Addreess = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}" ,
                Speciality = trainer.Speciality
            };
            return TrainerViewModel;
        }
        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model , CancellationToken ct)
        {
            var EmailExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email);
            var phoneexist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone);

            if ( phoneexist || EmailExist )
                return false;

            var newTrainer = new Trainer()
            {
                Name = model.Name ,
                Phone = model.Phone ,
                Email = model.Email ,
                Gender = model.Gender ,
                Speciality = model.Speciality ,
                DateOfBirth = model.DateOfBirth ,

                Address = new Address
                {
                    City = model.City ,
                    BuildingNumber = model.BuildingNumber ,
                    Street = model.Street ,
                }
            };

            _unitOfWork.GetRepository<Trainer>().Add(newTrainer);
            var Result = await _unitOfWork.SaveChangeAsync(ct);
            return Result > 0;

        }
        public async Task<TrainerToUpdateViewModel?> GetTrainertoUpdateAsync(int id , CancellationToken ct)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id , ct: ct);
            if ( trainer == null )
                return null;

            var TrainertoUpdate = new TrainerToUpdateViewModel()
            {
                Name = trainer.Name ,
                Email = trainer.Email ,
                Gender = trainer.Gender ,
                Phone = trainer.Phone ,
                DateOfBirth = trainer.DateOfBirth ,
                Speciality = trainer.Speciality ,
                BuildingNumber = trainer.Address.BuildingNumber ,
                City = trainer.Address.City ,
                Street = trainer.Address.Street ,
            };
            return TrainertoUpdate;
        }


        public async Task<bool> UpdateTrainerAsync(int id , TrainerToUpdateViewModel model , CancellationToken ct)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id , ct: ct);
            var EmailExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != id);
            var phoneexist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != id);

            if ( phoneexist || EmailExist )
                return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Speciality = model.Speciality;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.City = model.City;
            trainer.Address.Street = model.Street;
            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var Result = await _unitOfWork.SaveChangeAsync(ct);
            return Result > 0;

        }
        public async Task<bool> DeleteTrainerAsync(int id , CancellationToken ct)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id , ct: ct);
            if ( trainer == null ) return false;

            var hasFutureSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(t => t.TrainerId == id && t.EndDate > DateTime.Now , ct);
            if ( hasFutureSessions )
                return false;

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var Result = await _unitOfWork.SaveChangeAsync(ct);
            return Result > 0;
        }
    }
}
