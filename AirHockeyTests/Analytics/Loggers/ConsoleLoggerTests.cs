using NUnit.Framework;
using AirHockey.Analytics.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Analytics.Loggers.Tests
{
    [TestFixture]
    public class ConsoleLoggerTests
    {
        private ConsoleLogger _consoleLogger;
        private StringWriter _consoleOutput;

        [SetUp]
        public void SetUp()
        {
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);

            _consoleLogger = new ConsoleLogger();
        }

        [TearDown]
        public void TearDown()
        {
            _consoleOutput.Dispose();
            Console.SetOut(Console.Out);
        }

        [Test]
        public void Constructor_WhenCalled_PrintsInitializationMessage()
        {
            string output = _consoleOutput.ToString();
            Assert.That(output, Does.Contain("Initialized Game Analytics console logger."), "Initialization message was not printed.");
        }

        [Test]
        public void PrintEvent_PrintsEventName()
        {
            string eventName = "GoalScored";
            var eventData = new Dictionary<string, object>();

            _consoleLogger.PrintEvent(eventName, eventData);

            string output = _consoleOutput.ToString();
            Assert.That(output, Does.Contain($"Event: {eventName}"), "Event name was not printed.");
        }

        [Test]
        public void PrintEvent_PrintsEventData_Player()
        {
            string eventName = "GoalScored";
            var eventData = new Dictionary<string, object> { { "Player", "Player1" } };

            _consoleLogger.PrintEvent(eventName, eventData);

            string output = _consoleOutput.ToString();
            Assert.That(output, Does.Contain("  Player: Player1"), "Event data (Player) was not printed correctly.");
        }

        [Test]
        public void PrintEvent_PrintsEventData_Score()
        {
            string eventName = "GoalScored";
            var eventData = new Dictionary<string, object> { { "Score", 1 } };

            _consoleLogger.PrintEvent(eventName, eventData);

            string output = _consoleOutput.ToString();
            Assert.That(output, Does.Contain("  Score: 1"), "Event data (Score) was not printed correctly.");
        }
    }
}
