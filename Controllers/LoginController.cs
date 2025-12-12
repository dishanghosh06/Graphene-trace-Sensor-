using Microsoft.AspNetCore.Mvc;
using ClinicianDashboard.Services;

namespace ClinicianDashboard.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginService _loginService;

        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var clinicianName = _loginService.ValidateClinician(email, password);

            if (clinicianName == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View("Index");
            }

            // Save the doctor's name for next page
            TempData["ClinicianName"] = clinicianName;

            // Go to patient selection page
            return RedirectToAction("Index", "Clinician");
        }
    }
}
