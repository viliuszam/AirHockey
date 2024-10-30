using NUnit.Framework;
using AirHockey.Effects.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirHockey.Actors;

namespace AirHockey.Effects.Behaviors.Tests
{
    [TestFixture]
    public class WindBehaviorTests
    {
        private WindBehavior _windBehavior;
        private Player _player;
        private Puck _puck;

        [SetUp]
        public void SetUp()
        {
            _windBehavior = new WindBehavior();

            _player = new Player("1", "Red", 0f, 0f, "Player1", new Room("Room1"));
            _puck = new Puck(); // Initialize puck with default constructor
        }

        [Test]
        public void Execute_OnPlayer_IncreasesVelocity()
        {
            var initialVelocityX = _player.VelocityX;
            var initialVelocityY = _player.VelocityY;

            _windBehavior.Execute(_player);

            Assert.That(_player.VelocityX, Is.Not.EqualTo(initialVelocityX), "Player's velocity in X direction should change.");
            Assert.That(_player.VelocityY, Is.Not.EqualTo(initialVelocityY), "Player's velocity in Y direction should change.");
        }

        [Test]
        public void Execute_OnPuck_IncreasesVelocity()
        {
            var initialVelocityX = _puck.VelocityX;
            var initialVelocityY = _puck.VelocityY;

            _windBehavior.Execute(_puck);

            Assert.That(_puck.VelocityX, Is.Not.EqualTo(initialVelocityX), "Puck's velocity in X direction should change.");
            Assert.That(_puck.VelocityY, Is.Not.EqualTo(initialVelocityY), "Puck's velocity in Y direction should change.");
        }

        [Test]
        public void Revert_OnPlayer_DoesNotChangeVelocity()
        {
            var initialVelocityX = _player.VelocityX;
            var initialVelocityY = _player.VelocityY;

            _windBehavior.Execute(_player); // Apply wind behavior
            var windedVelocityX = _player.VelocityX;
            var windedVelocityY = _player.VelocityY;

            _windBehavior.Revert(_player);

            Assert.That(_player.VelocityX, Is.EqualTo(windedVelocityX), "Player's velocity in X direction should remain unchanged after revert.");
            Assert.That(_player.VelocityY, Is.EqualTo(windedVelocityY), "Player's velocity in Y direction should remain unchanged after revert.");
        }

        [Test]
        public void Revert_OnPuck_DoesNotChangeVelocity()
        {
            _windBehavior.Execute(_puck); // Apply wind behavior
            var windedVelocityX = _puck.VelocityX;
            var windedVelocityY = _puck.VelocityY;

            _windBehavior.Revert(_puck);

            Assert.That(_puck.VelocityX, Is.EqualTo(windedVelocityX), "Puck's velocity in X direction should remain unchanged after revert.");
            Assert.That(_puck.VelocityY, Is.EqualTo(windedVelocityY), "Puck's velocity in Y direction should remain unchanged after revert.");
        }
        [Test]
        public void Identifier_ReturnsExpectedPrefixAndAngle()
        {
            var result = _windBehavior.Identifier();

            StringAssert.StartsWith("WIND,", result, "The identifier should start with 'WIND,'.");

            var anglePart = result.Substring(5);
            Assert.That(float.TryParse(anglePart, out _), Is.True, "The identifier should contain a valid angle after 'WIND,'.");
        }

    }
}