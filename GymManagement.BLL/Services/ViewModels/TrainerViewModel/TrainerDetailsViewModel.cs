using GymManagement.DAL.Data.Models.Enums;

namespace GymManagement.BLL.Services.ViewModels.TrainerViewModel
{
    public class TrainerDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public DateOnly DateOfBirth { get; set; }
        public string Addreess { get; set; } = default!;
        public Speciality Speciality { get; set; }
    }
}
