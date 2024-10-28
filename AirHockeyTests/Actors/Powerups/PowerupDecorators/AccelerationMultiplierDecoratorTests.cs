using AirHockey.Actors.Powerups.PowerupDecorators;
using AirHockey.Actors.Powerups;
using AirHockey.Actors;
using NUnit.Framework;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AirHockey.Actors.Powerups.PowerupDecorators.Tests
{
    [TestFixture]
    public class AccelerationMultiplierDecoratorTests
    {
        private class MockPowerup : Powerup
        {
            public MockPowerup(float x, float y, int id) : base(x, y, id) { }

            public override void Activate(Player player) { }
            public override Powerup CloneShallow() => new MockPowerup(X, Y, Id);
            public override Powerup CloneDeep() => new MockPowerup(X, Y, Id);

            public override void Update()
            {
            }
        }

        private Player _player;
        private Powerup _mockPowerup;

        [SetUp]
        public void Setup()
        {
            _player = new Player("TestPlayer", "red", 50, 50, "Tester", new Room("TestRoom"));
            _player.Acceleration = 1f;
            _mockPowerup = new MockPowerup(0, 0, 1);
        }

        [Test]
        public void Activate_IncreasesPlayerAcceleration()
        {
            var decorator = new AccelerationMultiplierDecorator(_mockPowerup, 2.0f, 1.0f, new Timer());
            decorator.Activate(_player);

            Assert.AreEqual(2.0f, _player.Acceleration, "Player acceleration should be multiplied by the factor");
        }

        [Test]
        public void ResetAcceleration_RestoresOriginalAcceleration()
        {
            var timer = new Timer();
            var decorator = new AccelerationMultiplierDecorator(_mockPowerup, 2.0f, 1.0f, timer);

            decorator.Activate(_player);
            decorator.ResetAcceleration(_player);

            Assert.AreEqual(1.0f, _player.Acceleration, "Player acceleration should reset after duration ends");
        }

        [Test]
        public void CloneShallow_ReturnsNewInstance()
        {
            var decorator = new AccelerationMultiplierDecorator(_mockPowerup, 2.0f, 1.0f);
            var shallowClone = decorator.CloneShallow();

            Assert.IsInstanceOf<AccelerationMultiplierDecorator>(shallowClone);
            Assert.AreNotSame(decorator, shallowClone);
        }

        [Test]
        public void CloneDeep_ReturnsNewInstance()
        {
            var decorator = new AccelerationMultiplierDecorator(_mockPowerup, 2.0f, 1.0f);
            var deepClone = decorator.CloneDeep();

            Assert.IsInstanceOf<AccelerationMultiplierDecorator>(deepClone);
            Assert.AreNotSame(decorator, deepClone);
        }
    }
}