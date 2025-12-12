using System.Collections.Generic;

namespace ClinicianDashboard.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Status { get; set; } = "Stable";
        public int PeakPressure { get; set; }

        // 🔑 REQUIRED for CSV mapping
        public string CsvPrefix { get; set; } = "";

        public List<string> Comments { get; set; } = new();
        public List<PressureReading> PressureHistory { get; set; } = new();
    }

    public class PressureReading
    {
        public string Label { get; set; } = "";
        public int Pressure { get; set; }
    }
}
