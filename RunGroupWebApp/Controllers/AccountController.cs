using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Data;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,ApplicationDbContext applicationDbContext)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _applicationDbContext=applicationDbContext;

        }
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
                
            }
            var user = await _userManager.FindByEmailAsync(login.EmailAddress);
            if (user != null)
            {
                //Kullanıcı bulundu şimdi şifreyi kontrol ediyoruz..
                var passwordCheck=await _userManager.CheckPasswordAsync(user,login.Password);
                if (passwordCheck)
                {
                    //Şifte doğru ve giriş yapıyoruz.
                    var result = await _signInManager.PasswordSignInAsync(user,login.Password, false, false);
                    if(result.Succeeded)
                    {
                        return RedirectToAction("Index", "Club");
                    }
                }
                //Şifre yanlış olduğunda
                TempData["Error"] = "Wrong credentials. please try again";
                return View(login);
            }
            //Kullanıcı bulunamadı
            TempData["Error"] = "Please try again";
            return View(login);
        }
        [HttpGet]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            var newUser = new AppUser()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (newUserResponse.Succeeded)
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            return RedirectToAction("Login","Account");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Race");
        }
    }
}
