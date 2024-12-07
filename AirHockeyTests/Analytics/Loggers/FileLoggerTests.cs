﻿using NUnit.Framework;
using AirHockey.Analytics.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AirHockey.Analytics.Loggers.Tests
{
    [TestFixture]
    public class FileLoggerTests
    {
        private string _testDirectory;
        private FileLogger _fileLogger;

        [SetUp]
        public void SetUp()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _fileLogger = new FileLogger(_testDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Test]
        public void Constructor_CreatesInitializationLogFile()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string expectedFilePath = Path.Combine(_testDirectory, $"initialization_{timestamp}.log");

            AssertFileExistsWithContents(expectedFilePath);
        }

        [Test]
        public void InitializationLogFile_ContainsExpectedContent()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string expectedFilePath = Path.Combine(_testDirectory, $"initialization_{timestamp}.log");

            var logEntry = ReadLogFile(expectedFilePath);

            Assert.That(logEntry.ContainsKey("Timestamp"), Is.True, "Timestamp not found in initialization log.");
            Assert.That(logEntry.ContainsKey("Initialized"), Is.True, "Initialization status not found in log.");
            Assert.That(logEntry["Initialized"].ToString(), Is.EqualTo("True"), "Initialization flag not set correctly.");
        }

        [Test]
        public void WriteToFile_CreatesRoomLogFile()
        {
            string roomCode = "ABCD";
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string expectedFilePath = Path.Combine(_testDirectory, $"room_{roomCode}_{timestamp}.log");

            _fileLogger.WriteToFile(roomCode, "TestEvent", new Dictionary<string, object>());

            AssertFileExistsWithContents(expectedFilePath);
        }

        [Test]
        public void RoomLogFile_ContainsEventName()
        {
            string roomCode = "ABCD";
            string eventName = "GoalScored";
            string expectedFilePath = GenerateRoomLogFile(roomCode, eventName, new Dictionary<string, object>());

            var logEntry = ReadLogFile(expectedFilePath);

            Assert.That(logEntry.ContainsKey("EventName"), Is.True, "EventName not found in room log.");
            Assert.That(logEntry["EventName"].ToString(), Is.EqualTo(eventName), "EventName is not as expected.");
        }

        [Test]
        public void RoomLogFile_ContainsEventData()
        {
            string roomCode = "ABCD";
            var eventData = new Dictionary<string, object> { { "Player", "Player1" }, { "Score", 1 } };
            string expectedFilePath = GenerateRoomLogFile(roomCode, "GoalScored", eventData);

            var logEntry = ReadLogFile(expectedFilePath);
            var eventDataEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(logEntry["EventData"].ToString());

            Assert.That(eventDataEntry["Player"].ToString(), Is.EqualTo("Player1"), "Player data is not as expected.");
            Assert.That(eventDataEntry["Score"].ToString(), Is.EqualTo("1"), "Score data is not as expected.");
        }

        private void AssertFileExistsWithContents(string filePath)
        {
            Assert.That(File.Exists(filePath), Is.True, $"Log file '{filePath}' was not created.");
            Assert.That(File.ReadAllText(filePath), Is.Not.Empty, $"Log file '{filePath}' is empty.");
        }

        private Dictionary<string, object> ReadLogFile(string filePath)
        {
            string fileContents = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContents);
        }

        private string GenerateRoomLogFile(string roomCode, string eventName, Dictionary<string, object> eventData)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string expectedFilePath = Path.Combine(_testDirectory, $"room_{roomCode}_{timestamp}.log");
            _fileLogger.WriteToFile(roomCode, eventName, eventData);
            return expectedFilePath;
        }
    }
}