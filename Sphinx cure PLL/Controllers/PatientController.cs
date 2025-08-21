using Microsoft.AspNetCore.Mvc;
using Sphinx_cure_.BLL.Helper;
using Sphinx_cure_.BLL.ModelVM.Patient;
using Sphinx_cure_.BLL.Services.Abstractions;
using Sphinx_cure_.DAL.Entities;

namespace Sphinx_cure_PLL.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // GET: /Patients
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

        // GET: /Patients/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Patients/Create
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

            TempData["Success"] = message;
            return RedirectToAction(nameof(Index));
        }
        


        // GET: /Patients/Edit/5
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var (status, message, patient) = await _patientService.GetPatientByIdAsync(id);
        //    if (!status) return RedirectToAction(nameof(Index));

        //    var editModel = new PatientDTO
        //    {
        //        Id = patient.Id,
        //        Name = patient.Name,
        //        FilePath = patient.FilePath
        //    };

        //    return View(editModel);
        //}

        //// POST: /Patients/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, PatientDTO model)
        //{
        //    if (id != model.Id) return BadRequest();

        //    if (!ModelState.IsValid) return View(model);

        //    var (status, message) = await _patientService.UpdatePatientAsync(model);
        //    if (!status)
        //    {
        //        ModelState.AddModelError("", message);
        //        return View(model);
        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        // GET: /Patients/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var (status, message) = await _patientService.DeletePatientAsync(id);
            if (!status) TempData["Error"] = message;
            return RedirectToAction(nameof(Index));
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

            // تحديد نوع الملف حسب الامتداد
            var ext = Path.GetExtension(filePath).ToLower();
            string contentType = ext switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };

            return File(memory, contentType); // بدون اسم ملف = المتصفح يحاول عرضه مباشرة
        }

    }
}
