using Microsoft.AspNetCore.Identity;

namespace GymManagement.DAL.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirestName { get; set; } = default!;
        public string LastName { get; set; } = default!;
    }
}
