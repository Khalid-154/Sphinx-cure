using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sphinx_cure_.BLL.ModelVM.Account;
using Sphinx_cure_.DAL.Entities;

namespace Sphinx_cure_PLL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        
        public AccountController (UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                // Create user without email if not provided
                var user = new User()
                {
                    UserName = registerVM.UserName
                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);

                if (result.Succeeded)
                {
                    // Auto login after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Patient");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(registerVM);
        }

        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginVM login)
        {
            if (ModelState.IsValid)
            {
                // Find user by username ONLY (not by email)
                var user = await _userManager.FindByNameAsync(login.UserName);

                if (user != null)
                {
                    // Check password
                    var passwordValid = await _userManager.CheckPasswordAsync(user, login.Password);

                    if (passwordValid)
                    {
                        // Sign in the user
                        await _signInManager.SignInAsync(user, login.RememberMe);

                        return RedirectToAction("Dashboard", "Home");
                    }
                }

                // If we get here, login failed
                ModelState.AddModelError("", "Invalid username or password");
            }

            return View(login);
        }

    }
}
