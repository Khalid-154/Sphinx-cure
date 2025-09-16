using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sphinx_cure_.BLL.Helper;
using Sphinx_cure_.BLL.ModelVM.Patient;
using Sphinx_cure_.BLL.Services.Abstractions;
using Sphinx_cure_.DAL.Entities;
using Sphinx_cure_PLL.Hubs;

namespace Sphinx_cure_PLL.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IHubContext<NotificationHub> _hubContext;


        public PatientController(IPatientService patientService, IHubContext<NotificationHub> hubContext)
        {
            _patientService = patientService;
            _hubContext = hubContext;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (status, message, patients) = await _patientService.GetAllPatientsAsync();
            if (!status)
            {
                TempData["Error"] = message;
                return View(new List<PatientDTO>());
            }
            return View(patients);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var (status, message, patient) = await _patientService.GetPatientByIdAsync(id);
            if (!status) return RedirectToAction("Index");
            return View(patient);
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddPatientVM model, IFormFile file)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (status, message) = await _patientService.AddPatientAsync(model, file);

            if (!status)
            {
                ViewBag.Error = message;
                return View(model);
            }

            await _hubContext.Clients.All.SendAsync("RefreshPatients", "Create");
            TempData["Success"] = message;
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> UpdateFile(int id)
        {
            var (status, message, patient) = await _patientService.GetPatientByIdAsync(id);
            if (!status || patient == null)
                return RedirectToAction(nameof(Index));

            ViewBag.PatientId = id;
            ViewBag.PatientName = patient.Name;

            return View(new UpdatePatientFileVM());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFile(int id, UpdatePatientFileVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (status, message) = await _patientService.UpdatePatientAsync(id, model);

            if (!status)
            {
                ViewBag.Error = message;
                return View(model);
            }

            await _hubContext.Clients.All.SendAsync("RefreshPatients", "Update");


            TempData["Success"] = message;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (status, message) = await _patientService.DeletePatientAsync(id);

            if (!status)
                return StatusCode(500, message);

            await _hubContext.Clients.All.SendAsync("RefreshPatients", "Delete");

            return Ok(message); 
        }


        // GET: /Patients/Search?name=Ali
        //public async Task<IActionResult> Search(string name)
        //{
        //    var (status, message, patients) = await _patientService.SearchPatientsByNameAsync(name);
        //    if (!status)
        //    {
        //        TempData["Error"] = message;
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View("Index", patients);
        //}

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var (status, message, patient) = await _patientService.GetPatientByIdAsync(id);
            if (!status || patient == null || string.IsNullOrEmpty(patient.FilePath))
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", patient.FilePath);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var contentType = "application/octet-stream";
            return File(memory, contentType, Path.GetFileName(filePath));
        }

        [HttpGet]
        public async Task<IActionResult> ViewFile(int id)
        {
            var (status, message, patient) = await _patientService.GetPatientByIdAsync(id);
            if (!status || patient == null || string.IsNullOrEmpty(patient.FilePath))
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", patient.FilePath);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var ext = Path.GetExtension(filePath).ToLower();
            string contentType = ext switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };

            return File(memory, contentType); 
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientTable()
        {
            var (status, message, patients) = await _patientService.GetAllPatientsAsync();
            if (!status)
                return PartialView("_PatientTable", new List<PatientDTO>());

            return PartialView("_PatientTable", patients);
        }


        
        [HttpGet]
        public async Task<IActionResult> SearchNames(string term)
        {
            var (status, message, patients) = await _patientService.SearchPatientsByNameAsync(term);

            if (!status || patients == null || patients.Count == 0)
                return Json(new List<string>());

            return Json(patients.Select(p => p.Name).ToList());
        }

        // بترجع جدول مريض واحد أو أكتر
        [HttpGet]
        public async Task<IActionResult> Search(string name)
        {
            var (status, message, patients) = await _patientService.SearchPatientsByNameAsync(name);

            if (!status || patients == null || patients.Count == 0)
                return PartialView("_PatientTable", new List<PatientDTO>());

            return PartialView("_PatientTable", patients);
        }





    }
}
