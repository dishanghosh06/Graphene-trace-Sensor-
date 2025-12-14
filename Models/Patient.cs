using System.Collections.Generic;

namespace ClinicianDashboard.Models
{
    public class Patient
    {
        // Unique patient identifier
        public int Id { get; set; }

        // Patient full name
        public string Name { get; set; } = string.Empty;

        // Current clinical status (Stable, High Pressure, etc.)
        public string Status { get; set; } = "Stable";

        // Latest recorded peak pressure value
        public int PeakPressure { get; set; }

        // CSV filename prefix used to load heatmap data
        public string CsvPrefix { get; set; } = string.Empty;

        // Clinician comments (stored in memory)
        public List<string> Comments { get; set; } = new List<string>();

        // Pressure trend data used for graph display
        public List<PressureReading> PressureHistory { get; set; } = new List<PressureReading>();
    }

    // Represents a single pressure reading on the trend graph
    public class PressureReading
    {
        // Label shown on the graph (timestamp or date)
        public string Label { get; set; } = string.Empty;

        // Pressure value
        public int Pressure { get; set; }
    }
}
