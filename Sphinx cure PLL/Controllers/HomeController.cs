using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sphinx_cure_.BLL.Services.Abstractions;
using Sphinx_cure_PLL.Models;

namespace Sphinx_cure_PLL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPatientService _patientService;

        public HomeController(ILogger<HomeController> logger, IPatientService patientService)
        {
            _logger = logger;
            _patientService = patientService;
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
