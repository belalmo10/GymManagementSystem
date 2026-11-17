using GymManagement.DAL.Data.Models.Enums;

namespace GymManagement.BLL.Services.ViewModels.TrainerViewModel
{
    public class TrainerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public Speciality Specialization { get; set; } = default!;
    }
}
