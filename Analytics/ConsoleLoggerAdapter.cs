using AirHockey.Analytics.Loggers;

namespace AirHockey.Analytics
{
    public class ConsoleLoggerAdapter : IGameAnalytics
    {
        private readonly ConsoleLogger consoleLogger = new ConsoleLogger();

        public void LogEvent(string roomCode, string eventName, Dictionary<string, object> eventData)
        {
            Console.WriteLine($"Room Code: {roomCode}");
            consoleLogger.PrintEvent(eventName, eventData);
        }
    }

}
