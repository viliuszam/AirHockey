using NUnit.Framework;
using AirHockey.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AirHockey.Analytics.Tests
{
    [TestFixture]
    public class FileLoggerAdapterTests
    {
        private string _testDirectory;
        private FileLoggerAdapter _fileLoggerAdapter;

        [SetUp]
        public void SetUp()
        {
            // Create a temporary directory for testing
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            // Initialize FileLoggerAdapter with the test directory
            _fileLoggerAdapter = new FileLoggerAdapter(_testDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the test directory and files
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Test]
        public void LogEvent_CreatesLogFile()
        {
            // Arrange
            string roomCode = "ABCD";
            string eventName = "GoalScored";
            var eventData = new Dictionary<string, object> { { "Player", "Player1" }, { "Score", 1 } };

            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string expectedFileName = $"room_{roomCode}_{timestamp}.log";
            string expectedFilePath = Path.Combine(_testDirectory, expectedFileName);

            // Act
            _fileLoggerAdapter.LogEvent(roomCode, eventName, eventData);

            // Assert
            Assert.That(File.Exists(expectedFilePath), Is.True, "Log file was not created.");
        }

        [Test]
        public void LogEvent_WritesCorrectEventName()
        {
            // Arrange
            string roomCode = "ABCD";
            string eventName = "GoalScored";
            var eventData = new Dictionary<string, object> { { "Player", "Player1" }, { "Score", 1 } };
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string expectedFilePath = Path.Combine(_testDirectory, $"room_{roomCode}_{timestamp}.log");

            // Act
            _fileLoggerAdapter.LogEvent(roomCode, eventName, eventData);

            // Assert
            string fileContents = File.ReadAllText(expectedFilePath);
            var logEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContents);
            Assert.That(logEntry["EventName"].ToString(), Is.EqualTo(eventName), "Event name does not match.");
        }

        [Test]
        public void LogEvent_WritesCorrectEventData()
        {
            // Arrange
            string roomCode = "ABCD";
            string eventName = "GoalScored";
            var eventData = new Dictionary<string, object> { { "Player", "Player1" }, { "Score", 1 } };
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string expectedFilePath = Path.Combine(_testDirectory, $"room_{roomCode}_{timestamp}.log");

            // Act
            _fileLoggerAdapter.LogEvent(roomCode, eventName, eventData);

            // Assert
            string fileContents = File.ReadAllText(expectedFilePath);
            var logEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContents);
            var eventDataEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(logEntry["EventData"].ToString());

            Assert.That(eventDataEntry["Player"].ToString(), Is.EqualTo("Player1"), "Player data does not match.");
            Assert.That(eventDataEntry["Score"].ToString(), Is.EqualTo("1"), "Score data does not match.");
        }

        [Test]
        public void LogEvent_WritesTimestamp()
        {
            // Arrange
            string roomCode = "ABCD";
            string eventName = "GoalScored";
            var eventData = new Dictionary<string, object> { { "Player", "Player1" }, { "Score", 1 } };
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string expectedFilePath = Path.Combine(_testDirectory, $"room_{roomCode}_{timestamp}.log");

            // Act
            _fileLoggerAdapter.LogEvent(roomCode, eventName, eventData);

            // Assert
            string fileContents = File.ReadAllText(expectedFilePath);
            var logEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContents);
            Assert.That(logEntry.ContainsKey("Timestamp"), Is.True, "Timestamp is missing from log entry.");
        }
    }
}