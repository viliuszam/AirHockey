using NUnit.Framework;
using AirHockey.Effects.Areas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirHockey.Actors;
using Moq;

namespace AirHockey.Effects.Areas.Tests
{
    [TestFixture]
    public class LocalFieldEffectTests
    {
        private Mock<IEffectBehavior> _mockBehavior;
        private Room _room;
        private LocalFieldEffect _localFieldEffect;
        private const float EffectRadius = 50f;

        [SetUp]
        public void SetUp()
        {
            // Mock the effect behavior
            _mockBehavior = new Mock<IEffectBehavior>();

            // Create a room with a puck
            _room = new Room("Room1")
            {
                Puck = new Puck { X = 20f, Y = 20f } // In range
            };


            // Add players to the room
            _room.Players.Add(new Player("1", "Red", 30f, 30f, "Player1", _room)); // In range
            _room.Players.Add(new Player("2", "Blue", 100f, 100f, "Player2", _room)); // Out of range

            // Initialize LocalFieldEffect with a radius that includes Player1 and the puck
            _localFieldEffect = new LocalFieldEffect(1, _mockBehavior.Object, 10f, 25f, 25f, 50f, false);
        }

        [Test]
        public void ApplyEffect_WhenEntityInRange_ExecutesBehavior()
        {
            // Act
            _localFieldEffect.ApplyEffect(_room);

            // Assert
            _mockBehavior.Verify(b => b.Execute(It.IsAny<Entity>()), Times.Exactly(2), "Expected Execute to be called twice.");
        }

        [Test]
        public void ApplyEffect_WhenEntityOutOfRange_DoesNotExecuteBehavior()
        {
            // Act
            _localFieldEffect.ApplyEffect(_room);

            // Assert
            _mockBehavior.Verify(b => b.Execute(It.Is<Player>(p => p.Nickname == "Player2")), Times.Never);
        }

        [Test]
        public void RemoveEffect_WhenCalled_RevertsEffectOnAffectedEntities()
        {
            // Arrange
            _localFieldEffect.ApplyEffect(_room); // Apply effect first to affect entities

            // Act
            _localFieldEffect.RemoveEffect(_room);

            // Assert
            _mockBehavior.Verify(b => b.Revert(It.IsAny<Entity>()), Times.Exactly(2)); // 2 affected entities
        }

        [Test]
        public void ApplyEffect_WhenReapplyIsTrue_ReappliesEffectOnEntities()
        {
            // Arrange
            _localFieldEffect = new LocalFieldEffect(1, _mockBehavior.Object, 10f, 50f, 50f, EffectRadius, true);

            // Act
            _localFieldEffect.ApplyEffect(_room);
            _localFieldEffect.ApplyEffect(_room); // Apply again

            // Assert
            _mockBehavior.Verify(b => b.Execute(It.IsAny<Entity>()), Times.Exactly(4)); // Should still affect all in-range entities
        }

        [Test]
        public void ApplyEffect_WhenEntityMovesOutOfRange_RevertsEffect()
        {
            // Arrange
            _localFieldEffect.ApplyEffect(_room); // Apply effect first to affect entities

            // Simulate moving Player1 out of range
            _room.Players[0].X = 100f;
            _room.Players[0].Y = 100f;

            // Act
            _localFieldEffect.ApplyEffect(_room); // Reapply effect to check if revert happens

            // Assert
            _mockBehavior.Verify(b => b.Revert(It.IsAny<Entity>()), Times.Once); // Only Player1 should have been reverted
        }
        [Test]
        public void GetBehavior_ReturnsCorrectBehavior()
        {
            // Act
            var behavior = _localFieldEffect.GetBehavior();

            // Assert
            Assert.That(behavior, Is.SameAs(_mockBehavior.Object), "GetBehavior should return the correct behavior instance.");
        }
    }
}