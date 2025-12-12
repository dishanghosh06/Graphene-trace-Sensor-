
using System.Globalization;
using ClinicianDashboard.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;


namespace ClinicianDashboard.Services
{
    public class PatientService
    {
        private readonly string _csvFolder;
        private readonly Dictionary<string, int> _csvIndexTracker = new();


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
            _csvFolder = Path.Combine(env.ContentRootPath, "Data", "CSV");
        }

        public List<Patient> GetPatients() => _patients;

        // ✔ Nullable-safe
        public Patient? GetPatient(int id)
        {
            return _patients.FirstOrDefault(p => p.Id == id);
        }

        //  LOAD NEXT CSV FOR HEATMAP (cycles through files)
public int[] LoadNextHeatmapValues(string prefix)
{
    var files = Directory.GetFiles(_csvFolder, $"{prefix}_*.csv")
                         .OrderBy(f => f)
                         .ToList();

    if (files.Count == 0)
        return Enumerable.Repeat(0, 1024).ToArray();

    // Initialize index if missing
    if (!_csvIndexTracker.ContainsKey(prefix))
        _csvIndexTracker[prefix] = 0;

    // Pick next CSV
    var file = files[_csvIndexTracker[prefix]];

    // Move to next (loop)
    _csvIndexTracker[prefix] =
        (_csvIndexTracker[prefix] + 1) % files.Count;

    var values = new List<int>();

    foreach (var line in File.ReadAllLines(file))
    {
        foreach (var c in line.Split(','))
        {
            if (int.TryParse(c, out int v))
                values.Add(v);
        }
    }

    // Always return exactly 1024 values
    return values
        .Concat(Enumerable.Repeat(0, 1024))
        .Take(1024)
        .ToArray();
}
// 📈 LOAD PRESSURE TREND (one value per CSV)
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
            foreach (var c in line.Split(','))
            {
                if (int.TryParse(c, out int v) && v > 0)
                    values.Add(v);
            }
        }

        if (values.Count == 0) continue;

        var datePart = Path.GetFileNameWithoutExtension(file).Split('_')[1];
        double avg = values.Average();

        result.Add((datePart, avg));
    }

    return result;
}



        public void UpdateStatus(int id, string status, int pressure)
        {
            var p = GetPatient(id);
            if (p == null) return;

            p.Status = status;
            p.PeakPressure = pressure;

            p.PressureHistory.Add(new PressureReading
            {
                Label = DateTime.Now.ToShortTimeString(),
                Pressure = pressure
            });
        }

        public void AddComment(int id, string comment)
        {
            var p = GetPatient(id);
            if (p == null) return;

            p.Comments.Add(comment);
        }
        // 🚨 CHECK ALERT STATUS
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

    }
}
