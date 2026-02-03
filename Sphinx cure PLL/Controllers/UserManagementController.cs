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
    public class UserManagementController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        [HttpGet]
        public IActionResult Index()
        {
            var users = _userManager.Users.Where(u => !u.IsDeleted).ToList();
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(currentUserId))
            {
                var currentUser = _userManager.FindByIdAsync(currentUserId).Result;
                ViewBag.IsAdmin = currentUser?.Role == UserRole.Admin;
            }
            else
            {
                ViewBag.IsAdmin = false;
            }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string userId, string newPassword)
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
                TempData["Error"] = "Only admins can reset passwords.";
                return RedirectToAction("Index");
            }

            // Find the target user
            var targetUser = await _userManager.FindByIdAsync(userId);
            if (targetUser == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Prevent admin from resetting their own password
            if (targetUser.Id == currentUserId)
            {
                TempData["Error"] = "You cannot reset your own password.";
                return RedirectToAction("Index");
            }

            // Validate new password
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["Error"] = "Password must be at least 6 characters long.";
                return RedirectToAction("Index");
            }

            // Remove old password and set new one
            var removePasswordResult = await _userManager.RemovePasswordAsync(targetUser);
            if (!removePasswordResult.Succeeded)
            {
                TempData["Error"] = "Failed to remove old password.";
                return RedirectToAction("Index");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(targetUser, newPassword);
            if (addPasswordResult.Succeeded)
            {
                TempData["Success"] = $"Password for {targetUser.UserName} has been reset successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to set new password.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
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
                TempData["Error"] = "Only admins can edit users.";
                return RedirectToAction("Index");
            }

            // Find the target user
            var targetUser = await _userManager.FindByIdAsync(userId);
            if (targetUser == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Prevent admin from editing themselves
            if (targetUser.Id == currentUserId)
            {
                TempData["Error"] = "You cannot edit your own account.";
                return RedirectToAction("Index");
            }

            // Fix for CS9035: Set required 'Password' property in CreateUserVM initializer in EditUser GET action
            var model = new CreateUserVM
            {
                UserName = targetUser.UserName ?? string.Empty, // Ensure non-null assignment
                Role = targetUser.Role.ToString(),
                Password = string.Empty // Set to empty string since password is not edited here
            };

            ViewBag.Roles = new[] { "Admin", "Viewer", "Editor" };
            ViewBag.UserId = userId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string userId, CreateUserVM model)
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
                TempData["Error"] = "Only admins can edit users.";
                return RedirectToAction("Index");
            }

            // Find the target user
            var targetUser = await _userManager.FindByIdAsync(userId);
            if (targetUser == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Prevent admin from editing themselves
            if (targetUser.Id == currentUserId)
            {
                TempData["Error"] = "You cannot edit your own account.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Role))
            {
                ModelState.AddModelError("", "Username and role are required.");
                ViewBag.Roles = new[] { "Admin", "Viewer", "Editor" };
                ViewBag.UserId = userId;
                return View(model);
            }

            // Validate role
            if (model.Role != "Admin" && model.Role != "Viewer" && model.Role != "Editor")
            {
                ModelState.AddModelError("", "Invalid role selected.");
                ViewBag.Roles = new[] { "Admin", "Viewer", "Editor" };
                ViewBag.UserId = userId;
                return View(model);
            }

            var userRole = model.Role switch
            {
                "Admin" => UserRole.Admin,
                "Viewer" => UserRole.Viewer,
                "Editor" => UserRole.Editor,
                _ => UserRole.Viewer,
            };

            // Update user details
            targetUser.UserName = model.UserName;
            typeof(User)
                .GetProperty("Role", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
                ?.SetValue(targetUser, userRole);

            var result = await _userManager.UpdateAsync(targetUser);
            if (result.Succeeded)
            {
                // Update role if changed
                var currentRoles = await _userManager.GetRolesAsync(targetUser);
                await _userManager.RemoveFromRolesAsync(targetUser, currentRoles);
                await _userManager.AddToRoleAsync(targetUser, model.Role);

                TempData["Success"] = $"User {model.UserName} updated successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                ViewBag.Roles = new[] { "Admin", "Viewer", "Editor" };
                ViewBag.UserId = userId;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
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
                TempData["Error"] = "Only admins can delete users.";
                return RedirectToAction("Index");
            }

            // Find the target user
            var targetUser = await _userManager.FindByIdAsync(userId);
            if (targetUser == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Prevent admin from deleting themselves
            if (targetUser.Id == currentUserId)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction("Index");
            }

            // Soft delete the user
            targetUser.MarkAsDeleted();


            var result = await _userManager.UpdateAsync(targetUser);
            if (result.Succeeded)
            {
                TempData["Success"] = $"User {targetUser.UserName} has been deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete the user.";
            }

            return RedirectToAction("Index");
        }
    }
}
