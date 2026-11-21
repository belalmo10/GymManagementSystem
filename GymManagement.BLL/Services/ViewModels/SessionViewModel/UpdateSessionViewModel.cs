using System.ComponentModel.DataAnnotations;

namespace GymManagement.BLL.Services.ViewModels.SessionViewModel
{
    public class UpdateSessionViewModel
    {
        [Required(ErrorMessage = "Description Is Required")]
        [StringLength(500 , MinimumLength = (10) , ErrorMessage = "Category Must Be Between 10 To 500 Char")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "StartDate Is Required")]
        [Display(Name = "Start Date & Time")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate Is Required")]
        [Display(Name = "End Date & Time")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Trainer Is Required")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }
    }
}
