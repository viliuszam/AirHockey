using Microsoft.AspNetCore.Mvc.Diagnostics;
using Newtonsoft.Json;

namespace AirHockey.Analytics.Loggers
{
    public class FileLogger
    {
        private readonly string fileDirectory;

        public FileLogger(string fileDirectory)
        {
            this.fileDirectory = fileDirectory;
            InitializeFileLogger();
        }

        public void WriteToFile(string roomCode, string eventName, Dictionary<string, object> eventData)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string fileName = $"room_{roomCode}_{timestamp}.log";
            string filePath = Path.Combine(fileDirectory, fileName);

            var logEntry = new
            {
                Timestamp = DateTime.Now,
                EventName = eventName,
                EventData = eventData
            };

            string jsonLogEntry = JsonConvert.SerializeObject(logEntry, Formatting.Indented);
            using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.WriteLine(jsonLogEntry);
            }


        }

        private void InitializeFileLogger()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string fileName = $"initialization_{timestamp}.log";
            string filePath = Path.Combine(fileDirectory, fileName);
            var logEntry = new
            {
                Timestamp = timestamp,
                Initialized = true
            };
            string jsonLogEntry = JsonConvert.SerializeObject(logEntry, Formatting.Indented);
            File.AppendAllText(filePath, jsonLogEntry + Environment.NewLine);
        }
    }
}
