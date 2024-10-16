namespace AirHockey.Analytics
{
    public interface IGameAnalytics
    {
        void LogEvent(string roomCode, string eventName, Dictionary<string, object> eventData);
    }
}
