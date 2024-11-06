using NUnit.Framework;
using AirHockey.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirHockey.Actors;
using System.Collections.Concurrent;

namespace AirHockey.Managers.Tests
{
    [TestFixture]
    public class GameSessionManagerTests
    {
        private GameSessionManager _manager;

        [SetUp]
        public void SetUp()
        {
            _manager = GameSessionManager.Instance;
            _manager.ActiveRooms.Clear();
            _manager.ActiveGames.Clear();
        }

        [Test]
        public void Instance_ReturnsSameInstanceEachTime()
        {
            var instance1 = GameSessionManager.Instance;
            var instance2 = GameSessionManager.Instance;

            Assert.That(instance1, Is.SameAs(instance2), "GameSessionManager.Instance does not return a singleton instance.");
        }

        [Test]
        public void AddRoom_AddsRoomToActiveRooms()
        {
            var room = new Room("Room1");

            _manager.AddRoom(room);

            Assert.That(_manager.ActiveRooms.ContainsKey("Room1"), Is.True, "Room was not added to ActiveRooms.");
            Assert.That(_manager.GetRoom("Room1"), Is.EqualTo(room), "Retrieved room does not match the added room.");
        }

        [Test]
        public void RemoveRoom_RemovesRoomAndEndsGame()
        {
            var room = new Room("Room1");
            _manager.AddRoom(room);
            _manager.StartNewGame(room);

            _manager.RemoveRoom("Room1");

            Assert.That(_manager.ActiveRooms.ContainsKey("Room1"), Is.False, "Room was not removed from ActiveRooms.");
            Assert.That(_manager.ActiveGames.ContainsKey("Room1"), Is.False, "Game was not ended when room was removed.");
        }

        [Test]
        public void RoomExists_ReturnsTrueIfRoomExists()
        {
            var room = new Room("Room1");
            _manager.AddRoom(room);

            bool exists = _manager.RoomExists("Room1");

            Assert.That(exists, Is.True, "RoomExists did not return true for an existing room.");
        }

        [Test]
        public void RoomExists_ReturnsFalseIfRoomDoesNotExist()
        {
            bool exists = _manager.RoomExists("NonExistentRoom");

            Assert.That(exists, Is.False, "RoomExists did not return false for a non-existing room.");
        }

        [Test]
        public void StartNewGame_AddsGameToActiveGames()
        {
            var room = new Room("Room1");
            _manager.AddRoom(room);

            _manager.StartNewGame(room);

            Assert.That(_manager.ActiveGames.ContainsKey("Room1"), Is.True, "Game was not added to ActiveGames.");
        }

        [Test]
        public void EndGame_RemovesGameFromActiveGames()
        {
            var room = new Room("Room1");
            _manager.AddRoom(room);
            _manager.StartNewGame(room);

            _manager.EndGame("Room1");

            Assert.That(_manager.ActiveGames.ContainsKey("Room1"), Is.False, "Game was not removed from ActiveGames.");
        }

        [Test]
        public void GetRoom_ReturnsCorrectRoom()
        {
            var room = new Room("Room1");
            _manager.AddRoom(room);

            var retrievedRoom = _manager.GetRoom("Room1");

            Assert.That(retrievedRoom, Is.EqualTo(room), "GetRoom did not return the expected room.");
        }

        [Test]
        public void GetRoom_ReturnsNullIfRoomDoesNotExist()
        {
            var retrievedRoom = _manager.GetRoom("NonExistentRoom");

            Assert.That(retrievedRoom, Is.Null, "GetRoom did not return null for a non-existent room.");
        }

        [Test]
        public void GetGame_ReturnsCorrectGame()
        {
            var room = new Room("Room1");
            _manager.AddRoom(room);
            _manager.StartNewGame(room);

            var retrievedGame = _manager.GetGame("Room1");

            Assert.That(retrievedGame.Room, Is.EqualTo(room), "GetGame did not return the expected game.");
        }

        [Test]
        public void GetGame_ReturnsNullIfGameDoesNotExist()
        {
            var retrievedGame = _manager.GetGame("NonExistentRoom");

            Assert.That(retrievedGame, Is.Null, "GetGame did not return null for a non-existent game.");
        }

        // -----------------------------------------
        // Thread safety tests

        [Test]
        public void AddRoom_ShouldBeThreadSafe()
        {
            var manager = GameSessionManager.Instance;
            var concurrentRooms = new ConcurrentDictionary<string, Room>();

            Parallel.For(0, 1000, i =>
            {
                var room = new Room($"ROOM{i}");
                manager.AddRoom(room);
                concurrentRooms.TryAdd(room.RoomCode, room);
            });

            foreach (var room in concurrentRooms)
            {
                Assert.True(manager.RoomExists(room.Key));
            }
        }

        [Test]
        public void RemoveRoom_ShouldBeThreadSafe()
        {
            var manager = GameSessionManager.Instance;
            var concurrentRooms = new ConcurrentDictionary<string, Room>();

            Parallel.For(0, 1000, i =>
            {
                var room = new Room($"ROOM{i}");
                manager.AddRoom(room);
                concurrentRooms.TryAdd(room.RoomCode, room);
            });

            Parallel.ForEach(concurrentRooms, room =>
            {
                manager.RemoveRoom(room.Key);
            });

            foreach (var room in concurrentRooms)
            {
                Assert.False(manager.RoomExists(room.Key));
            }
        }

        [Test]
        public void StartNewGame_ShouldBeThreadSafe()
        {
            var manager = GameSessionManager.Instance;
            var concurrentRooms = new ConcurrentDictionary<string, Room>();

            Parallel.For(0, 1000, i =>
            {
                var room = new Room($"ROOM{i}");
                manager.AddRoom(room);
                concurrentRooms.TryAdd(room.RoomCode, room);
            });

            Parallel.ForEach(concurrentRooms, room =>
            {
                manager.StartNewGame(room.Value);
            });

            foreach (var room in concurrentRooms)
            {
                Assert.NotNull(manager.GetGame(room.Key));
            }
        }

        [Test]
        public void EndGame_ShouldBeThreadSafe()
        {
            var manager = GameSessionManager.Instance;
            var concurrentRooms = new ConcurrentDictionary<string, Room>();

            Parallel.For(0, 1000, i =>
            {
                var room = new Room($"ROOM{i}");
                manager.AddRoom(room);
                manager.StartNewGame(room);
                concurrentRooms.TryAdd(room.RoomCode, room);
            });

            Parallel.ForEach(concurrentRooms, room =>
            {
                manager.EndGame(room.Key);
            });

            foreach (var room in concurrentRooms)
            {
                Assert.Null(manager.GetGame(room.Key));
            }
        }
    }
}