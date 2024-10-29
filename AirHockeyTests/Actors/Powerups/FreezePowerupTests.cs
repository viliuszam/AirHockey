using AirHockey.Actors.Powerups;
using AirHockey.Actors;
using NUnit.Framework;
using System.Timers;
using System.Numerics;

namespace AirHockey.Actors.Powerups.Tests
{
    [TestFixture]
    public class FreezePowerupTests
    {
        private class MockPlayer : Player
        {
            public MockPlayer(float maxSpeed, string id, Room room) : base(id, "red", 50, 50, "T"+id, room)
            {
                MaxSpeed = maxSpeed;
                VelocityX = 1;
                VelocityY = 1;
            }

        }

        private MockPlayer _player1;
        private MockPlayer _player2;
        private MockPlayer _player3;
        private FreezePowerup _freezePowerup;
        [SetUp]
        public void Setup()
        {
            var room = new Room("TestRoom");
            var room2 = new Room("TestRoom2");
            _player1 = new MockPlayer(5.0f, "TestPlayer1", room);
            _player2 = new MockPlayer(5.0f, "TestPlayer2", room);
            _player3 = new MockPlayer(5.0f, "TestPlayer3", room2);
            room.AddPlayer(_player1);
            room.AddPlayer(_player2);
            room2.AddPlayer(_player3);

            _freezePowerup = new FreezePowerup(0, 0, 1, 1.0f); // Freeze duration is 1 second
        }

        [Test]
        public void Activate_FreezesEnemyPlayer()
        {
            _freezePowerup.Activate(_player1);

            Assert.AreEqual(0, _player2.MaxSpeed, "Enemy player's MaxSpeed should be set to 0");
            Assert.AreEqual(0, _player2.VelocityX, "Enemy player's VelocityX should be set to 0");
            Assert.AreEqual(0, _player2.VelocityY, "Enemy player's VelocityY should be set to 0");
        }

        [Test]
        public void Activate_NoEnemyException()
        {
            Assert.Throws<InvalidOperationException>(() => _freezePowerup.Activate(_player3));
        }


        [Test]
        public void Timer_ResetEnemyPlayerSpeedAfterDuration()
        {
            _freezePowerup.Activate(_player1);

            System.Threading.Thread.Sleep((int)(_freezePowerup.FreezeDuration * 1000) + 50);

            Assert.AreEqual(5.0f, _player2.MaxSpeed, "Enemy player's MaxSpeed should be reset to original value after duration");
        }

        [Test]
        public void CloneDeep_CreatesIdenticalCopy()
        {
            var clone = (FreezePowerup)_freezePowerup.CloneDeep();

            Assert.AreEqual(_freezePowerup.X, clone.X);
            Assert.AreEqual(_freezePowerup.Y, clone.Y);
            Assert.AreEqual(_freezePowerup.Id, clone.Id);
            Assert.AreEqual(_freezePowerup.FreezeDuration, clone.FreezeDuration);
        }

        [Test]
        public void CloneShallow_CreatesIdenticalCopy()
        {
            var clone = (FreezePowerup)_freezePowerup.CloneShallow();

            Assert.AreEqual(_freezePowerup.X, clone.X);
            Assert.AreEqual(_freezePowerup.Y, clone.Y);
            Assert.AreEqual(_freezePowerup.Id, clone.Id);
            Assert.AreEqual(_freezePowerup.FreezeDuration, clone.FreezeDuration);
        }
    }
}