using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.ViewModels;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly SignInManager<AppIdentityUser> _signInManager;

        private readonly Context _context;
        public AccountController(UserManager<AppIdentityUser> userManager, SignInManager<AppIdentityUser> signInManager, Context context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new AppIdentityUser
            {
                UserName = model.email,
                Email = model.email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth
            };

            var result = await _userManager.CreateAsync(user, model.password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var employee = new Employee
            {
                UserID = user.Id,
                hireDate = DateTime.Now,
                Is_Active = true
            };
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: true);
            

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.email);

            if (user == null )
            {
                ModelState.AddModelError(string.Empty, "Sorry, we couldn't find an account with that email address.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.password, isPersistent: true, lockoutOnFailure: false);

            if(!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Sorry, we couldn't find an account with that email address.");
                return View(model);
            }
            if(result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Sorry, your account is locked out.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
