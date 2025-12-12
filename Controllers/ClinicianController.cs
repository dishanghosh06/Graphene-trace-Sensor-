using Microsoft.AspNetCore.Mvc;
using ClinicianDashboard.Services;

namespace ClinicianDashboard.Controllers
{
    public class ClinicianController : Controller
    {
        private readonly PatientService _patientService;

        public ClinicianController(PatientService patientService)
        {
            _patientService = patientService;
        }

        public IActionResult Index(int? id)
        {
            ViewBag.Patients = _patientService.GetPatients();
            ViewBag.SelectedPatient = id.HasValue
                ? _patientService.GetPatient(id.Value)
                : null;

            return View();
        }

        [HttpPost]
        public IActionResult UpdatePatient(int id, string status, int pressure)
        {
            _patientService.UpdateStatus(id, status, pressure);
            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult AddComment(int id, string commentText)
        {
            _patientService.AddComment(id, commentText);
            return RedirectToAction("Index", new { id });
        }

        // 🔥 REAL CSV HEATMAP ENDPOINT
        [HttpGet]
public IActionResult GetLatestPressure(int id)
{
    var patient = _patientService.GetPatient(id);
    if (patient == null)
        return Json(new int[1024]);

    var values = _patientService.LoadNextHeatmapValues(patient.CsvPrefix);
    return Json(values);
}
[HttpGet]
public IActionResult GetPressureTrend(int id)
{
    var patient = _patientService.GetPatient(id);
    if (patient == null)
        return Json(new { labels = new string[0], values = new double[0] });

    var data = _patientService.LoadPressureTrend(patient.CsvPrefix);

    return Json(new
    {
        labels = data.Select(d => d.label).ToArray(),
        values = data.Select(d => d.avgPressure).ToArray()
    });
}
[HttpGet]
public IActionResult GetAlertStatus(int id)
{
    var patient = _patientService.GetPatient(id);
    if (patient == null)
        return Json("No Data");

    var alert = _patientService.GetAlertStatus(patient.CsvPrefix);
    return Json(alert);
}


    }
}