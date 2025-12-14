using Microsoft.AspNetCore.Mvc;
using ClinicianDashboard.Services;
using System.Linq;

namespace ClinicianDashboard.Controllers
{
    
    /// Controller responsible for clinician dashboard functionality.
    /// Handles patient selection, updates, comments, heatmap data,
    /// pressure trend graph data, and alert status.
    
    public class ClinicianController : Controller
    {
        private readonly PatientService _patientService;

      
        /// Constructor with dependency injection of PatientService
       
        public ClinicianController(PatientService patientService)
        {
            _patientService = patientService;
        }
        /// Displays clinician dashboard and selected patient details
       
        public IActionResult Index(int? id)
        {
            ViewBag.Patients = _patientService.GetPatients();
            ViewBag.SelectedPatient = id.HasValue
                ? _patientService.GetPatient(id.Value)
                : null;

            return View();
        }

        /// Updates patient status and peak pressure
      
        [HttpPost]
        public IActionResult UpdatePatient(int id, string status, int pressure)
        {
            _patientService.UpdateStatus(id, status, pressure);
            return RedirectToAction("Index", new { id });
        }

        /// Saves clinician comments for a patient
     
        [HttpPost]
        public IActionResult AddComment(int id, string commentText)
        {
            _patientService.AddComment(id, commentText);
            return RedirectToAction("Index", new { id });
        }

        /// Returns live heatmap data (32x32 = 1024 values)
        /// Data cycles through CSV files to simulate real-time updates
    
        [HttpGet]
        public IActionResult GetLatestPressure(int id)
        {
            var patient = _patientService.GetPatient(id);
            if (patient == null)
            {
                // Return empty heatmap if patient not found
                return Json(new int[1024]);
            }

            var values = _patientService.LoadNextHeatmapValues(patient.CsvPrefix);
            return Json(values);
        }

        /// Returns pressure trend data for graph (hourly average)
   
        [HttpGet]
        public IActionResult GetPressureTrend(int id)
        {
            var patient = _patientService.GetPatient(id);
            if (patient == null)
            {
                return Json(new
                {
                    labels = new string[0],
                    values = new double[0]
                });
            }

            var data = _patientService.LoadPressureTrend(patient.CsvPrefix);

            return Json(new
            {
                labels = data.Select(d => d.label).ToArray(),
                values = data.Select(d => d.avgPressure).ToArray()
            });
        }

        /// Returns alert message based on latest pressure values
     
        [HttpGet]
        public IActionResult GetAlertStatus(int id)
        {
            var patient = _patientService.GetPatient(id);
            if (patient == null)
            {
                return Json("No Data Available");
            }

            var alert = _patientService.GetAlertStatus(patient.CsvPrefix);
            return Json(alert);
        }
    }
}
