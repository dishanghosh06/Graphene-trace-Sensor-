namespace ClinicianDashboard.Services
{
    
    /// Handles clinician authentication logic.
    
    
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

    
        /// Validates clinician credentials.
        /// If valid, returns the clinician name.
       
        
        public string? ValidateClinician(string email, string password)
        {
            // Simple password check
            if (_clinicians.ContainsKey(email) && password == "123")
            {
                return _clinicians[email];
            }

            return null;
        }
    }
}
