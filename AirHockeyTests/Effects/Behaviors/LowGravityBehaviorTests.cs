using NUnit.Framework;
using AirHockey.Effects.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirHockey.Actors;
using Moq;

namespace AirHockey.Effects.Behaviors.Tests
{
    [TestFixture]
    public class LowGravityBehaviorTests
    {
        private LowGravityBehavior _lowGravityBehavior;
        private Player _player;
        private Puck _puck;

        [SetUp]
        public void SetUp()
        {
            _lowGravityBehavior = new LowGravityBehavior();

            _player = new Player("1", "Red", 0f, 0f, "Player1", new Room("Room1"));
            _puck = new Puck(); // Initialize puck with default constructor
        }

        [Test]
        public void Execute_OnPlayer_SetsMassToHalf()
        {
            var originalMass = _player.Mass;
            _lowGravityBehavior.Execute(_player);

            Assert.That(_player.Mass, Is.EqualTo(originalMass / 2), "Player mass should be halved.");
        }

        [Test]
        public void Revert_OnPlayer_SetsMassBackToOriginal()
        {
            var originalMass = _player.Mass;
            _lowGravityBehavior.Execute(_player); // First apply low gravity
            _lowGravityBehavior.Revert(_player); // Revert the effect

            Assert.That(_player.Mass, Is.EqualTo(originalMass), "Player mass should revert back to original.");
        }

        [Test]
        public void Execute_OnPuck_SetsMassToHalf()
        {
            var originalMass = _puck.Mass;
            _lowGravityBehavior.Execute(_puck);

            Assert.That(_puck.Mass, Is.EqualTo(originalMass / 2), "Puck mass should be halved.");
        }

        [Test]
        public void Revert_OnPuck_SetsMassBackToOriginal()
        {
            var originalMass = _puck.Mass;
            _lowGravityBehavior.Execute(_puck); // First apply low gravity
            _lowGravityBehavior.Revert(_puck); // Revert the effect

            Assert.That(_puck.Mass, Is.EqualTo(originalMass), "Puck mass should revert back to original.");
        }
        [Test]
        public void Identifier_ReturnsExpectedIdentifier()
        {
            var result = _lowGravityBehavior.Identifier();

            Assert.That(result, Is.EqualTo("LOW_GRAVITY"), "The identifier should be 'LOW_GRAVITY'.");
        }
    }
}