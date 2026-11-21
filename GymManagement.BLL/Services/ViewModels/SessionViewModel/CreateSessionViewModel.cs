using System.ComponentModel.DataAnnotations;

namespace GymManagement.BLL.Services.ViewModels.SessionViewModel
{
    public class CreateSessionViewModel
    {
        [Required(ErrorMessage = "Description Is Required")]
        [StringLength(500 , MinimumLength = (10) , ErrorMessage = "Category Must Be Between 10 To 500 Char")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Capacity Is Required")]
        [Range(0 , 25 , ErrorMessage = "Capacity Must Be Between 0 And 25")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "StartDate Is Required")]
        [Display(Name = "Start Date & Time")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate Is Required")]
        [Display(Name = "End Date & Time")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Trainer Is Required")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Category Is Required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }


    }
}
