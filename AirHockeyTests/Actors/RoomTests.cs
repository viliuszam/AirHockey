using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Tests
{
    [TestFixture]
    public class RoomTests
    {
        private Room _room;
        private const string TEST_ROOM_CODE = "TEST123";
        private const string PLAYER_1_ID = "p1";
        private const string PLAYER_2_ID = "p2";
        private const string PLAYER_3_ID = "p3";

        [SetUp]
        public void Setup()
        {
            _room = new Room(TEST_ROOM_CODE);
        }

        [Test]
        public void Constructor_InitializesRoomCorrectly()
        {
            Assert.That(_room.RoomCode, Is.EqualTo(TEST_ROOM_CODE));
            Assert.That(_room.Players, Is.Not.Null);
            Assert.That(_room.Players, Is.Empty);
            Assert.That(_room.Walls, Is.Not.Null);
            Assert.That(_room.Walls, Is.Empty);
            Assert.That(_room.Powerups, Is.Not.Null);
            Assert.That(_room.Powerups, Is.Empty);
            Assert.That(_room.Puck, Is.Not.Null);
        }

        [Test]
        public void AddPlayer_WhenRoomNotFull_AddsPlayerSuccessfully()
        {
            var player = new Player(PLAYER_1_ID, "red", 50, 50, "player1", _room);

            _room.AddPlayer(player);

            Assert.That(_room.Players.Count, Is.EqualTo(1));
            Assert.That(_room.Players.First(), Is.EqualTo(player));
        }

        [Test]
        public void AddPlayer_WhenRoomIsFull_ThrowsException()
        {
            _room.AddPlayer(new Player(PLAYER_1_ID, "red", 50, 50, "player1", _room));
            _room.AddPlayer(new Player(PLAYER_2_ID, "blue", 450, 50, "player2", _room));
            var extraPlayer = new Player(PLAYER_3_ID, "green", 250, 50, "player3", _room);

            var exception = Assert.Throws<InvalidOperationException>(() => _room.AddPlayer(extraPlayer));
            Assert.That(exception.Message, Is.EqualTo("Room is already full."));
        }

        [Test]
        public void GetPlayerById_WhenPlayerExists_ReturnsCorrectPlayer()
        {
            var player = new Player(PLAYER_1_ID, "red", 50, 50, "player1", _room);
            _room.AddPlayer(player);

            var result = _room.GetPlayerById(PLAYER_1_ID);

            Assert.That(result, Is.EqualTo(player));
        }

        [Test]
        public void GetPlayerById_WhenPlayerDoesNotExist_ReturnsNull()
        {
            var result = _room.GetPlayerById(PLAYER_1_ID);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void RemovePlayer_WhenPlayerExists_RemovesPlayerSuccessfully()
        {
            var player = new Player(PLAYER_1_ID, "red", 50, 50, "player1", _room);
            _room.AddPlayer(player);

            _room.RemovePlayer(PLAYER_1_ID);

            Assert.That(_room.Players, Is.Empty);
        }

        [Test]
        public void RemovePlayer_WhenPlayerDoesNotExist_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => _room.RemovePlayer(PLAYER_1_ID));
        }

        [Test]
        public void IsRoomFull_WhenRoomHasTwoPlayers_ReturnsTrue()
        {
            _room.AddPlayer(new Player(PLAYER_1_ID, "red", 50, 50, "player1", _room));
            _room.AddPlayer(new Player(PLAYER_2_ID, "blue", 450, 50, "player2", _room));

            var result = _room.IsRoomFull();

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsRoomFull_WhenRoomHasLessThanTwoPlayers_ReturnsFalse()
        {
            _room.AddPlayer(new Player(PLAYER_1_ID, "red", 50, 50, "player1", _room));

            var result = _room.IsRoomFull();

            Assert.That(result, Is.False);
        }
    }
}
