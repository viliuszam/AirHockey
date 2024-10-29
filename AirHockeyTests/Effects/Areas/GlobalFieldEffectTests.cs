using NUnit.Framework;
using AirHockey.Effects.Areas;
using System;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirHockey.Actors;

namespace AirHockey.Effects.Areas.Tests
{
    [TestFixture]
    public class GlobalFieldEffectTests
    {
        private Mock<IEffectBehavior> _mockBehavior;
        private Room _mockRoom;
        private List<Player> _players;
        private Puck _mockPuck;
        private GlobalFieldEffect _globalFieldEffect;

        [SetUp]
        public void SetUp()
        {
            // Set up mocks
            _mockBehavior = new Mock<IEffectBehavior>();
            _mockPuck = new Puck();

            // Create the mock room and set the players' Room property
            _mockRoom = new Room("Room1")
            {
                Puck = _mockPuck
            };

            // Initialize players using the new constructor
            _players = new List<Player>
            {
                new Player("1", "Red", 0, 0, "Player1", _mockRoom),
                new Player("2", "Blue", 0, 0, "Player2", _mockRoom)
            };

            // Initialize GlobalFieldEffect
            _globalFieldEffect = new GlobalFieldEffect(1, _mockBehavior.Object, 5.0f, false);
        }

        [Test]
        public void ApplyEffect_WhenNotApplied_CallsExecuteOnEntities()
        {
            // Act
            _globalFieldEffect.ApplyEffect(_mockRoom);

            // Assert
            foreach (var player in _mockRoom.Players)
            {
                _mockBehavior.Verify(b => b.Execute(player), Times.Once);
            }
            _mockBehavior.Verify(b => b.Execute(_mockRoom.Puck), Times.Once);
        }

        [Test]
        public void ApplyEffect_WhenReapplyIsTrue_CallsExecuteOnEntitiesAgain()
        {
            // Arrange
            _globalFieldEffect = new GlobalFieldEffect(1, _mockBehavior.Object, 5.0f, true);
            _globalFieldEffect.ApplyEffect(_mockRoom); // First application

            // Act
            _globalFieldEffect.ApplyEffect(_mockRoom); // Second application

            // Assert
            foreach (var player in _mockRoom.Players)
            {
                _mockBehavior.Verify(b => b.Execute(player), Times.Exactly(2));
            }
            _mockBehavior.Verify(b => b.Execute(_mockRoom.Puck), Times.Exactly(2));
        }

        [Test]
        public void RemoveEffect_WhenApplied_CallsRevertOnEntities()
        {
            // Arrange
            _globalFieldEffect.ApplyEffect(_mockRoom); // Apply effect to set isApplied to true

            // Act
            _globalFieldEffect.RemoveEffect(_mockRoom);

            // Assert
            foreach (var player in _mockRoom.Players)
            {
                _mockBehavior.Verify(b => b.Revert(player), Times.Once);
            }
            _mockBehavior.Verify(b => b.Revert(_mockRoom.Puck), Times.Once);
        }

        [Test]
        public void RemoveEffect_WhenNotApplied_DoesNotCallRevert()
        {
            // Act
            _globalFieldEffect.RemoveEffect(_mockRoom);

            // Assert
            foreach (var player in _mockRoom.Players)
            {
                _mockBehavior.Verify(b => b.Revert(player), Times.Never);
            }
            _mockBehavior.Verify(b => b.Revert(_mockRoom.Puck), Times.Never);
        }
        [Test]
        public void GetBehavior_ReturnsCorrectBehavior()
        {
            // Act
            var behavior = _globalFieldEffect.GetBehavior();

            // Assert
            Assert.That(behavior, Is.SameAs(_mockBehavior.Object), "GetBehavior should return the correct behavior instance.");
        }
    }
}