using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Powerups.PowerupDecorators.Tests
{
    using NUnit.Framework;
    using System.Timers;

    [TestFixture]
    public class MassMultiplierDecoratorTests
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
            _player.Mass = 10;
            _mockPowerup = new MockPowerup(0, 0, 1);
        }

        [Test]
        public void Activate_IncreasesPlayerMass()
        {
            var decorator = new MassMultiplierDecorator(_mockPowerup, 1.5f, 1.0f, new Timer());
            decorator.Activate(_player);

            Assert.AreEqual(15.0f, _player.Mass, "Player mass should be multiplied by the factor");
        }

        [Test]
        public void ResetMass_RestoresOriginalMass()
        {
            var timer = new Timer();
            var decorator = new MassMultiplierDecorator(_mockPowerup, 1.5f, 1.0f, timer);

            decorator.Activate(_player);
            decorator.ResetMass(_player);

            Assert.AreEqual(10.0f, _player.Mass, "Player mass should reset after duration ends");
        }
    }
}
