namespace AirHockey.Analytics.Loggers
{
    public class ConsoleLogger
    {
        public ConsoleLogger()
        {
            InitializeConsoleLogger();
        }

        public void PrintEvent(string eventName, Dictionary<string, object> eventData)
        {
            Console.WriteLine($"Event: {eventName}");
            foreach (var kvp in eventData)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }
        }

        private void InitializeConsoleLogger()
        {
            Console.WriteLine("Initialized Game Analytics console logger.");
        }
    }
}
