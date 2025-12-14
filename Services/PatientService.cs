using System.Globalization;
using ClinicianDashboard.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;

namespace ClinicianDashboard.Services
{
    public class PatientService
    {
        // Folder where all CSV files are stored
        private readonly string _csvFolder;

        // Tracks which CSV file is currently being used per patient (for live heatmap cycling)
        private readonly Dictionary<string, int> _csvIndexTracker = new();

        // In-memory patient list (acts as application data store)
        private readonly List<Patient> _patients = new()
        {
            new Patient { Id = 1, Name = "Drake", CsvPrefix = "1c0df777" },
            new Patient { Id = 2, Name = "Tanjiro", CsvPrefix = "71e66ab3" },
            new Patient { Id = 3, Name = "Zenitsu", CsvPrefix = "543d4676" },
            new Patient { Id = 4, Name = "Kendrick", CsvPrefix = "d13043b3" },
            new Patient { Id = 5, Name = "Harry", CsvPrefix = "de0e9b2c" }
        };

        public PatientService(IWebHostEnvironment env)
        {
            // CSV files stored under /Data/CSV
            _csvFolder = Path.Combine(env.ContentRootPath, "Data", "CSV");
        }

        //  PATIENT ACCESS 

        public List<Patient> GetPatients() => _patients;

        public Patient? GetPatient(int id)
        {
            return _patients.FirstOrDefault(p => p.Id == id);
        }

        // LIVE HEATMAP 
        // Cycles through all CSV files for a patient (simulates real-time updates)
        public int[] LoadNextHeatmapValues(string prefix)
        {
            var files = Directory.GetFiles(_csvFolder, $"{prefix}_*.csv")
                                 .OrderBy(f => f)
                                 .ToList();

            if (!files.Any())
                return Enumerable.Repeat(0, 1024).ToArray();

            // Initialize index if first time
            if (!_csvIndexTracker.ContainsKey(prefix))
                _csvIndexTracker[prefix] = 0;

            // Select next CSV file
            var file = files[_csvIndexTracker[prefix]];

            // Advance index (loop back when finished)
            _csvIndexTracker[prefix] =
                (_csvIndexTracker[prefix] + 1) % files.Count;

            var values = new List<int>();

            foreach (var line in File.ReadAllLines(file))
            {
                foreach (var cell in line.Split(','))
                {
                    if (int.TryParse(cell.Trim(), out int v))
                        values.Add(v);
                }
            }

            // Always return EXACTLY 1024 values (32×32 grid)
            return values
                .Concat(Enumerable.Repeat(0, 1024))
                .Take(1024)
                .ToArray();
        }

        
        // One average pressure value per CSV file
        public List<(string label, double avgPressure)> LoadPressureTrend(string prefix)
        {
            var files = Directory.GetFiles(_csvFolder, $"{prefix}_*.csv")
                                 .OrderBy(f => f)
                                 .ToList();

            var result = new List<(string, double)>();

            foreach (var file in files)
            {
                var values = new List<int>();

                foreach (var line in File.ReadAllLines(file))
                {
                    foreach (var cell in line.Split(','))
                    {
                        if (int.TryParse(cell.Trim(), out int v) && v > 0)
                            values.Add(v);
                    }
                }

                if (!values.Any())
                    continue;

                // Extract date from filename
                var datePart = Path.GetFileNameWithoutExtension(file).Split('_')[1];
                double average = values.Average();

                result.Add((datePart, average));
            }

            return result;
        }

        // ALERT LOGIC 
        public string GetAlertStatus(string prefix)
        {
            var trend = LoadPressureTrend(prefix);

            if (!trend.Any())
                return "No Data";

            var latest = trend.Last().avgPressure;

            if (latest >= 180)
                return "CRITICAL ALERT: Extremely High Pressure";

            if (latest >= 150)
                return "Warning: High Pressure";

            return "Normal";
        }

        // PATIENT UPDATES
        public void UpdateStatus(int id, string status, int pressure)
        {
            var patient = GetPatient(id);
            if (patient == null) return;

            patient.Status = status;
            patient.PeakPressure = pressure;

            patient.PressureHistory.Add(new PressureReading
            {
                Label = DateTime.Now.ToShortTimeString(),
                Pressure = pressure
            });
        }

        // COMMENTS
        public void AddComment(int id, string comment)
        {
            var patient = GetPatient(id);
            if (patient == null) return;

            patient.Comments.Add(comment);
        }
    }
}
