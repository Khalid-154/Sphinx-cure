using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sphinx_cure_.BLL.Services.Abstractions;
using Sphinx_cure_.DAL.Entities;
using Sphinx_cure_PLL.Hubs;
using Sphinx_cure_PLL.Models;

namespace Sphinx_cure_PLL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPatientService _patientService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, IPatientService patientService, IHubContext<NotificationHub> hubContext, UserManager<User> userManager)
        {
            _logger = logger;
            _patientService = patientService;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("SignIn", "Account");
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var (status, message, patients) = await _patientService.GetAllPatientsAsync();
            ViewBag.PatientCount = status ? patients.Count : 0;
            ViewBag.RecentPatients = status ? patients.OrderByDescending(p => p.Id).Take(5).ToList() : new List<Sphinx_cure_.BLL.ModelVM.Patient.PatientDTO>();

            // Get current user information
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var currentUser = await _userManager.FindByIdAsync(userId);
                if (currentUser != null)
                {
                    ViewBag.CurrentUserName = currentUser.UserName;
                    ViewBag.CurrentUserRole = currentUser.Role.ToString();
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> TestNotification()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "This is a test notification!");
            return Content("Notification sent!");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
