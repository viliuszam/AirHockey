using AirHockey.Analytics.Loggers;

namespace AirHockey.Analytics
{
    public class FileLoggerAdapter : IGameAnalytics
    {
        private readonly FileLogger _fileLogger;

        public FileLoggerAdapter(string fileDirectory)
        {
            _fileLogger = new FileLogger(fileDirectory);
        }

        public void LogEvent(string roomCode, string eventName, Dictionary<string, object> eventData)
        {
            _fileLogger.WriteToFile(roomCode, eventName, eventData);
        }
    }
}
