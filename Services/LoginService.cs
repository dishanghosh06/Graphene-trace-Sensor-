namespace ClinicianDashboard.Services
{
    public class LoginService
    {
        private readonly Dictionary<string, string> _clinicians =
            new Dictionary<string, string>()
            {
                { "jerry@hospital.com", "Dr. Jerry" },
                { "jack@hospital.com", "Dr. Jack" },
                { "linda@hospital.com", "Dr. Linda" }
            };

        public string? ValidateClinician(string email, string password)
        {
            if (_clinicians.ContainsKey(email) && password == "123")
                return _clinicians[email];

            return null;
        }
    }
}
