using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.ViewModels.PlanViewModel
{
    public class UpdatePlanViewModel : BaseEntity
    {
        public string? PlanName { get; set; }
        [Required(ErrorMessage = "Description Is Required")]
        [StringLength(200,MinimumLength =5,ErrorMessage = "Description must be between 5 And 200 Characters")]
        public string Description { get; set; } = default!;
        [Required(ErrorMessage ="Duration Is Required")]
        [Range(1,365,ErrorMessage ="Duration must be Between 1 to 365 Days")]
        public int DurationDays { get; set; }
        [Required(ErrorMessage = "Price Is Required")]
        [Range(0.1,10000,ErrorMessage ="price Must Be Greater than 0")]
        public decimal Price { get; set; }
    }
}
