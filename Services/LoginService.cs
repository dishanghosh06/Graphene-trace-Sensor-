namespace ClinicianDashboard.Services
{
    /// <summary>
    /// Handles clinician authentication logic.
    /// This service validates clinician login credentials
    /// and returns the clinician's display name if valid.
    /// </summary>
    public class LoginService
    {
        // In-memory list of allowed clinicians (email → display name)
        // This replaces a database for simplicity in this project
        private readonly Dictionary<string, string> _clinicians =
            new Dictionary<string, string>()
            {
                { "jerry@hospital.com", "Dr. Jerry" },
                { "jack@hospital.com", "Dr. Jack" },
                { "linda@hospital.com", "Dr. Linda" }
            };

        /// <summary>
        /// Validates clinician credentials.
        /// If valid, returns the clinician name.
        /// If invalid, returns null.
        /// </summary>
        public string? ValidateClinician(string email, string password)
        {
            // Simple password check (demo purpose only)
            if (_clinicians.ContainsKey(email) && password == "123")
            {
                return _clinicians[email];
            }

            return null;
        }
    }
}
