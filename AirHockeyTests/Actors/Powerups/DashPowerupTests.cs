using AirHockey.Actors.Powerups;
using AirHockey.Actors;
using NUnit.Framework;
using System.Timers;


namespace AirHockey.Actors.Powerups.Tests
{
    [TestFixture]
    public class DashPowerupTests
    {
        private class MockPlayer : Player
        {
            public MockPlayer(float maxSpeed) : base("TestPlayer", "red", 50, 50, "Tester", new Room("TestRoom"))
            {
                MaxSpeed = maxSpeed;
                VelocityX = 1;
                VelocityY = 1;
            }
        }

        private MockPlayer _player;
        private DashPowerup _dashPowerup;

        [SetUp]
        public void Setup()
        {
            _player = new MockPlayer(5.0f);
            _dashPowerup = new DashPowerup(0, 0, 1, 8f, 0.25f);
        }

        [Test]
        public void Activate_ChangesPlayerSpeed()
        {
            _dashPowerup.Activate(_player);

            Assert.AreEqual(8.0f, _player.MaxSpeed, "Player MaxSpeed should be set to DashSpeed");
            Assert.AreEqual(8.0f, _player.VelocityX, "Player VelocityX should be scaled by DashSpeed");
            Assert.AreEqual(8.0f, _player.VelocityY, "Player VelocityY should be scaled by DashSpeed");
        }

        [Test]
        public void Timer_ResetPlayerSpeedAfterDuration()
        {
            _dashPowerup.Activate(_player);

            System.Threading.Thread.Sleep((int)(_dashPowerup.DashDuration * 1000) + 50);

            Assert.AreEqual(5.0f, _player.MaxSpeed, "Player MaxSpeed should be reset to original value after duration");
        }

        [Test]
        public void CloneDeep_CreatesIdenticalCopy()
        {
            var clone = (DashPowerup)_dashPowerup.CloneDeep();

            Assert.AreEqual(_dashPowerup.X, clone.X);
            Assert.AreEqual(_dashPowerup.Y, clone.Y);
            Assert.AreEqual(_dashPowerup.Id, clone.Id);
            Assert.AreEqual(_dashPowerup.DashSpeed, clone.DashSpeed);
            Assert.AreEqual(_dashPowerup.DashDuration, clone.DashDuration);
        }

        [Test]
        public void CloneShallow_CreatesIdenticalCopy()
        {
            var clone = (DashPowerup)_dashPowerup.CloneShallow();

            Assert.AreEqual(_dashPowerup.X, clone.X);
            Assert.AreEqual(_dashPowerup.Y, clone.Y);
            Assert.AreEqual(_dashPowerup.Id, clone.Id);
            Assert.AreEqual(_dashPowerup.DashSpeed, clone.DashSpeed);
            Assert.AreEqual(_dashPowerup.DashDuration, clone.DashDuration);
        }
    }
}