using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.SessionViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        #region Way 01
        //public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        //{
        //    var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(ct: ct);
        //    var booking = await _unitOfWork.GetRepository<Booking>().GetAllAsync(ct: ct);

        //    var result = new List<SessionViewModel>();
        //    foreach ( var session in sessions )
        //    {
        //        var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(session.TrainerId , ct: ct);
        //        var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId , ct: ct);

        //        var bookedCount = booking.Count(b => b.SessionId == session.Id);

        //        result.Add(new SessionViewModel
        //        {
        //            TrainerName = session.Trainer.Name ,
        //            Description = session.Description ,
        //            Capacity = session.Capacity ,
        //            StartDate = session.StartDate ,
        //            EndDate = session.EndDate ,
        //            Speciality = session.Trainer.Speciality ,
        //            AvailableSlots = session.Capacity - bookedCount
        //        });
        //    }
        //    return result;
        //} 
        #endregion
        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(ct);
            if ( sessions == null || !sessions.Any() )
                return null;

            var mappedSession = sessions.Select(s => new SessionViewModel
            {
                Id = s.Id ,
                Description = s.Description ,
                Capacity = s.Capacity ,
                CategoryName = s.Category.CategoryName ,
                EndDate = s.EndDate ,
                TrainerName = s.Trainer.Name ,
                StartDate = s.StartDate ,
                Speciality = s.Trainer.Speciality ,
            });
            foreach ( var session in mappedSession )
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfSlotsAsync(session.Id , ct);
            }

            return mappedSession;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model , CancellationToken ct = default)
        {
            if ( model.StartDate >= model.EndDate ) return Result.ValidationError("EndDate Must Be After StartDate");
            if ( model.StartDate <= DateTime.Now ) return Result.ValidationError("StartDate Must Be In The Future");
            if ( model.Capacity < 1 || model.Capacity > 25 ) return Result.ValidationError("Capacity Must Be Between 1 And 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if ( trainer == null ) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if ( category == null ) return Result.NotFound("Category Not Found");

            var isValid = Enum.TryParse<Speciality>(category.CategoryName , true , out var categorySpeciality);
            if ( !isValid || trainer.Speciality != categorySpeciality )
                return Result.ValidationError("Can Not Create This Session To This Trainer");


            var session = _mapper.Map<CreateSessionViewModel , Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);
            var result = await _unitOfWork.SaveChangeAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Create Session");
        }


        public async Task<IEnumerable<CategorySelectedViewModel>> GetCategoryFromDropDownAsync(CancellationToken ct)
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<CategorySelectedViewModel>>(categories);
        }

        public async Task<IEnumerable<TrainerSelectedViewModel>> GetTrainerFromDropDownAsync(CancellationToken ct)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<TrainerSelectedViewModel>>(trainers);
        }

        public async Task<Result<SessionViewModel>> GetSessionDetailsAsync(int id , CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategory(id , ct);
            if ( session == null ) return Result<SessionViewModel>.NotFound("Session Not Found");
            else
            {
                var sessionMapped = _mapper.Map<SessionViewModel?>(session);
                sessionMapped.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfSlotsAsync(id , ct);
                return Result<SessionViewModel>.Ok(sessionMapped);
            }
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id , CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id , ct);
            if ( session == null ) return Result<UpdateSessionViewModel>.NotFound("Session Not Found");

            if ( session.StartDate <= DateTime.Now ) return Result<UpdateSessionViewModel>.Fail("Can Not Update Session That Has Already Started Or Ended ");

            var bookCount = await _unitOfWork.SessionRepository.GetCountOfSlotsAsync(id , ct);
            if ( bookCount > 0 ) return Result<UpdateSessionViewModel>.Fail("Can not Update Session That Has Bookings");

            var sessionToUpdate = _mapper.Map<UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(sessionToUpdate);

        }

        public async Task<Result> UpdateSessionAsync(int id , UpdateSessionViewModel model , CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id , ct);

            if ( session == null ) return Result.NotFound("Session Not Found");

            if ( session.StartDate >= model.EndDate ) return Result.ValidationError("Can Not Edit Session Because EndDate Must Be After StartDate");

            if ( session.StartDate <= DateTime.Now ) return Result.ValidationError("Can Not Edit Session Because StartDate Must Be In The Future");

            var bookCount = await _unitOfWork.SessionRepository.GetCountOfSlotsAsync(id , ct);
            if ( bookCount > 0 ) return Result.Fail("Can Not Update Session That Has Bookings");

            if ( model.StartDate <= DateTime.Now ) return Result.ValidationError("StartDate Must Be In The Future");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if ( trainer == null ) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);

            var isValid = Enum.TryParse<Speciality>(category?.CategoryName , true , out var categorySpeciality);
            if ( !isValid || trainer.Speciality != categorySpeciality )
                return Result.ValidationError("Can Not Edit This Session To This Trainer");

            _mapper.Map(model , session);
            session.UpdatedAt = DateTime.Now;

            _unitOfWork.SessionRepository.Update(session);

            var result = await _unitOfWork.SaveChangeAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Edit Session");
        }

        public async Task<Result> DeleteSessionAsync(int id , CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id);

            if ( session == null ) return Result.NotFound("Session Not Found");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfSlotsAsync(id , ct);
            if ( bookingCount > 0 ) return Result.ValidationError("Cannot Remove Session That Have Bookings");

            if ( session.EndDate >= DateTime.Now ) return Result.ValidationError("Cannot Remove Session That Has No Ended Yet");

            _unitOfWork.SessionRepository.Delete(session);
            var result = await _unitOfWork.SaveChangeAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Remove Session");
        }
    }
}
