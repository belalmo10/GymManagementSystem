using GymManagement.BLL.Services.ViewModels.AccountViewModel;
using GymManagement.DAL.Data.Models;
using GymManagementSystem.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager ,
                                 SignInManager<ApplicationUser> signInManager ,
                                 ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model , CancellationToken ct = default)
        {
            if ( !ModelState.IsValid ) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if ( user is null || string.IsNullOrEmpty(user.UserName) )
            {
                ModelState.AddModelError("InvalidLogin" , "Invalid email or password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName , model.Password , model.RememberMe , lockoutOnFailure: true);

            if ( result.Succeeded )
            {
                _logger.LogInformation("User {UserId} signed in." , user.Id);
                return RedirectToAction(nameof(HomeController.Index) , "Home");
            }
            if ( result.IsLockedOut )
            {
                _logger.LogWarning("User {UserId} is locked out." , user.Id);
                ModelState.AddModelError("InvalidLogin" , "This Account Is Temporarily Locked. Try Again Later.");
            }
            else if ( result.IsNotAllowed )
            {
                ModelState.AddModelError("InvalidLogin" , "Sign-in is Not Allowed For This Account.");
            }
            else
            {
                ModelState.AddModelError("InvalidLogin" , "Invalid Email Or Password.");
            }
            return View(model);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

    }
}
