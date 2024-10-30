using NUnit.Framework;
using AirHockey.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Analytics.Tests
{
    [TestFixture]
    public class ConsoleLoggerAdapterTests
    {
        private ConsoleLoggerAdapter _consoleLoggerAdapter;
        private StringWriter _consoleOutput;

        [SetUp]
        public void SetUp()
        {
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);

            _consoleLoggerAdapter = new ConsoleLoggerAdapter();
        }

        [TearDown]
        public void TearDown()
        {
            _consoleOutput.Dispose();
            Console.SetOut(Console.Out);
        }

        [Test]
        public void LogEvent_PrintsRoomCode()
        {
            string roomCode = "ABCD";
            var eventData = new Dictionary<string, object>();

            _consoleLoggerAdapter.LogEvent(roomCode, "TestEvent", eventData);

            string output = _consoleOutput.ToString();
            Assert.That(output, Does.Contain($"Room Code: {roomCode}"), "Room code was not printed.");
        }

        [Test]
        public void LogEvent_PrintsEventName()
        {
            string eventName = "GoalScored";
            var eventData = new Dictionary<string, object>();

            _consoleLoggerAdapter.LogEvent("ABCD", eventName, eventData);

            string output = _consoleOutput.ToString();
            Assert.That(output, Does.Contain($"Event: {eventName}"), "Event name was not printed.");
        }

        [Test]
        public void LogEvent_PrintsEventData_Player()
        {
            var eventData = new Dictionary<string, object> { { "Player", "Player1" } };

            _consoleLoggerAdapter.LogEvent("ABCD", "GoalScored", eventData);

            string output = _consoleOutput.ToString();
            Assert.That(output, Does.Contain("  Player: Player1"), "Event data (Player) was not printed correctly.");
        }

        [Test]
        public void LogEvent_PrintsEventData_Score()
        {
            var eventData = new Dictionary<string, object> { { "Score", 1 } };

            _consoleLoggerAdapter.LogEvent("ABCD", "GoalScored", eventData);

            string output = _consoleOutput.ToString();
            Assert.That(output, Does.Contain("  Score: 1"), "Event data (Score) was not printed correctly.");
        }
    }
}