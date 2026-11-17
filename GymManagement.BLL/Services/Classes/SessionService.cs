using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.ViewModels.SessionViewModel;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
