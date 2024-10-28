using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AirHockey.Actors.Powerups.PowerupDecorators.Tests
{
    [TestFixture]
    public class SpeedMultiplierDecoratorTests
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
            _player.MaxSpeed = 20f;
            _mockPowerup = new MockPowerup(0, 0, 1);
        }

        [Test]
        public void Activate_IncreasesPlayerSpeed()
        {
            var decorator = new SpeedMultiplierDecorator(_mockPowerup, 2.0f, 1.0f, new Timer());
            decorator.Activate(_player);

            Assert.AreEqual(40.0f, _player.MaxSpeed, "Player speed should be multiplied by the factor");
        }

        [Test]
        public void ResetSpeed_RestoresOriginalSpeed()
        {
            var timer = new Timer();
            var decorator = new SpeedMultiplierDecorator(_mockPowerup, 2.0f, 1.0f, timer);

            decorator.Activate(_player);
            decorator.ResetSpeed(_player);

            Assert.AreEqual(20.0f, _player.MaxSpeed, "Player speed should reset after duration ends");
        }
    }
}
