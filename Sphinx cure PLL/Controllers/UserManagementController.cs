using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sphinx_cure_.BLL.ModelVM.Account;
using Sphinx_cure_.DAL.Entities;
using Sphinx_cure_.DAL.Enums;
using System.Security.Claims;

namespace Sphinx_cure_PLL.Controllers
{
    [Authorize()]
    public class UserManagementController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Check if current user is an admin
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["Error"] = "Unable to determine current user.";
                return RedirectToAction("Index");
            }
            var currentUser = _userManager.FindByIdAsync(currentUserId).Result;
            if (currentUser == null || currentUser.Role != UserRole.Admin)
            {
                TempData["Error"] = "Only admins can create new users.";
                return RedirectToAction("Index");
            }

            ViewBag.Roles = new[] { "Admin", "Viewer", "Editor" };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM model)
        {
            // Check if current user is an admin
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["Error"] = "Unable to determine current user.";
                return RedirectToAction("Index");
            }
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null || currentUser.Role != UserRole.Admin)
            {
                TempData["Error"] = "Only admins can create new users.";
                return RedirectToAction("Index");
            }

            string userName = model.UserName;
            string password = model.Password;
            string role = model.Role;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("", "All fields are required.");
                ViewBag.Roles = new[] { "Admin", "Viewer", "Editor" };
                return View();
            }

            // Validate role
            if (role != "Admin" && role != "Viewer" && role != "Editor")
            {
                ModelState.AddModelError("", "Invalid role selected.");
                ViewBag.Roles = new[] { "Admin", "Viewer", "Editor" };
                return View();
            }

            var userRole = role switch
            {
                "Admin" => UserRole.Admin,
                "Viewer" => UserRole.Viewer,
                "Editor" => UserRole.Editor,
                _ => UserRole.Viewer,
            };
            var user = new User(userRole)
            {
                UserName = userName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["Success"] = $"User {model.UserName} created successfully with {model.Role} role.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                ViewBag.Roles = new[] { "Admin", "Viewer", "Editor" };
                return View(model);
            }
        }
    }
}
