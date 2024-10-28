using NUnit.Framework;
using AirHockey.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirHockey.Actors;

namespace AirHockey.Managers.Tests
{
    [TestFixture]
    public class GameSessionManagerTests
    {
        private GameSessionManager _manager;

        [SetUp]
        public void SetUp()
        {
            // Reset the singleton instance before each test
            _manager = GameSessionManager.Instance;
            _manager.ActiveRooms.Clear();
            _manager.ActiveGames.Clear();
        }

        [Test]
        public void Instance_ReturnsSameInstanceEachTime()
        {
            // Act
            var instance1 = GameSessionManager.Instance;
            var instance2 = GameSessionManager.Instance;

            // Assert
            Assert.That(instance1, Is.SameAs(instance2), "GameSessionManager.Instance does not return a singleton instance.");
        }

        [Test]
        public void AddRoom_AddsRoomToActiveRooms()
        {
            // Arrange
            var room = new Room("Room1");

            // Act
            _manager.AddRoom(room);

            // Assert
            Assert.That(_manager.ActiveRooms.ContainsKey("Room1"), Is.True, "Room was not added to ActiveRooms.");
            Assert.That(_manager.GetRoom("Room1"), Is.EqualTo(room), "Retrieved room does not match the added room.");
        }

        [Test]
        public void RemoveRoom_RemovesRoomAndEndsGame()
        {
            // Arrange
            var room = new Room("Room1");
            _manager.AddRoom(room);
            _manager.StartNewGame(room);

            // Act
            _manager.RemoveRoom("Room1");

            // Assert
            Assert.That(_manager.ActiveRooms.ContainsKey("Room1"), Is.False, "Room was not removed from ActiveRooms.");
            Assert.That(_manager.ActiveGames.ContainsKey("Room1"), Is.False, "Game was not ended when room was removed.");
        }

        [Test]
        public void RoomExists_ReturnsTrueIfRoomExists()
        {
            // Arrange
            var room = new Room("Room1");
            _manager.AddRoom(room);

            // Act
            bool exists = _manager.RoomExists("Room1");

            // Assert
            Assert.That(exists, Is.True, "RoomExists did not return true for an existing room.");
        }

        [Test]
        public void RoomExists_ReturnsFalseIfRoomDoesNotExist()
        {
            // Act
            bool exists = _manager.RoomExists("NonExistentRoom");

            // Assert
            Assert.That(exists, Is.False, "RoomExists did not return false for a non-existing room.");
        }

        [Test]
        public void StartNewGame_AddsGameToActiveGames()
        {
            // Arrange
            var room = new Room("Room1");
            _manager.AddRoom(room);

            // Act
            _manager.StartNewGame(room);

            // Assert
            Assert.That(_manager.ActiveGames.ContainsKey("Room1"), Is.True, "Game was not added to ActiveGames.");
        }

        [Test]
        public void EndGame_RemovesGameFromActiveGames()
        {
            // Arrange
            var room = new Room("Room1");
            _manager.AddRoom(room);
            _manager.StartNewGame(room);

            // Act
            _manager.EndGame("Room1");

            // Assert
            Assert.That(_manager.ActiveGames.ContainsKey("Room1"), Is.False, "Game was not removed from ActiveGames.");
        }

        [Test]
        public void GetRoom_ReturnsCorrectRoom()
        {
            // Arrange
            var room = new Room("Room1");
            _manager.AddRoom(room);

            // Act
            var retrievedRoom = _manager.GetRoom("Room1");

            // Assert
            Assert.That(retrievedRoom, Is.EqualTo(room), "GetRoom did not return the expected room.");
        }

        [Test]
        public void GetRoom_ReturnsNullIfRoomDoesNotExist()
        {
            // Act
            var retrievedRoom = _manager.GetRoom("NonExistentRoom");

            // Assert
            Assert.That(retrievedRoom, Is.Null, "GetRoom did not return null for a non-existent room.");
        }

        [Test]
        public void GetGame_ReturnsCorrectGame()
        {
            // Arrange
            var room = new Room("Room1");
            _manager.AddRoom(room);
            _manager.StartNewGame(room);

            // Act
            var retrievedGame = _manager.GetGame("Room1");

            // Assert
            Assert.That(retrievedGame.Room, Is.EqualTo(room), "GetGame did not return the expected game.");
        }

        [Test]
        public void GetGame_ReturnsNullIfGameDoesNotExist()
        {
            // Act
            var retrievedGame = _manager.GetGame("NonExistentRoom");

            // Assert
            Assert.That(retrievedGame, Is.Null, "GetGame did not return null for a non-existent game.");
        }
    }
}