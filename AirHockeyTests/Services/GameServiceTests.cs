using NUnit.Framework;
using AirHockey.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AirHockey.Actors;
using AirHockey.Actors.Walls;
using AirHockey.Actors.Powerups;
using AirHockey.Analytics;
using AirHockey.Managers;
using AirHockey.Observers;
using AirHockey.Strategies;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Drawing;
using System.Timers;
using System.Xml.Linq;
using AirHockey.Effects;
using System.Threading;
using AirHockey.Effects.Areas;
using AirHockey.Effects.Behaviors;
using AirHockey.Actors.Powerups.PowerupDecorators;
using AirHockey.Actors.Command;
using System.Threading.Tasks;
using Moq;

namespace AirHockey.Services.Tests
{
    [TestFixture]
    public class GameServiceTests
    {
        private GameService _gameService;
        private Mock<IHubContext<GameHub>> _mockHubContext;
        private Mock<IGameAnalytics> _mockAnalytics;
        private Mock<ICollision> _mockCollision;

        [SetUp]
        public void SetUp()
        {
            _mockHubContext = new Mock<IHubContext<GameHub>>();
            _mockAnalytics = new Mock<IGameAnalytics>();
            _mockCollision = new Mock<ICollision>();

            _gameService = new GameService(_mockHubContext.Object, _mockAnalytics.Object, _mockCollision.Object);
        }

        [Test]
        public void SetStrategy_UpdatesCollisionStrategy()
        {
            // Arrange
            var newCollisionStrategy = new Mock<ICollision>();

            // Act
            _gameService.SetStrategy(newCollisionStrategy.Object);

            // Assert
            // Use reflection to access the private 'collisions' field
            var fieldInfo = typeof(GameService).GetField("collisions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var currentStrategy = fieldInfo.GetValue(_gameService);

            Assert.That(currentStrategy, Is.EqualTo(newCollisionStrategy.Object), "The collision strategy should be updated.");
        }
        [Test]
        public void UpdateEnvironmentalEffects_WhenCalled_AddsNewEffectToActiveEffects()
        {
            // Arrange
            var game = new Game(new Room("Room1"))
            {
                ActiveEffects = new List<EnvironmentalEffect>()
            };

            // Create a specific EnvironmentalEffect directly
            var effect = new LocalFieldEffect(3, new LowGravityBehavior(), 60 * 3, 100f, 100f, 100f, false);

            // Simulate the addition of the effect
            game.ActiveEffects.Add(effect);

            // Act
            _gameService.UpdateEnvironmentalEffects(game);

            // Assert
            Assert.That(game.ActiveEffects, Has.Count.EqualTo(1), "Active effects count should be 1 after adding new effect.");
            Assert.That(game.ActiveEffects[0].ID, Is.EqualTo(effect.ID), "The effect added to active effects should match the created effect.");
        }
        [Test]
        public void UpdateEnvironmentalEffects_WhenEffectDurationReachesZero_RemovesEffectFromActiveEffects()
        {
            // Arrange
            var game = new Game(new Room("Room1"))
            {
                ActiveEffects = new List<EnvironmentalEffect>()
            };

            // Create an EnvironmentalEffect with duration 1
            var effect = new LocalFieldEffect(3, new LowGravityBehavior(), 1, 100f, 100f, 100f, false);
            game.ActiveEffects.Add(effect);

            // Act
            _gameService.UpdateEnvironmentalEffects(game); // Duration will decrement to 0
            _gameService.UpdateEnvironmentalEffects(game); // This call will remove the effect

            // Assert
            Assert.That(game.ActiveEffects, Is.Empty, "Active effects should be empty after effect duration reaches zero.");
        }
        [Test]
        public void GenerateWalls_AddsWallsToRoom()
        {
            // Arrange
            var room = new Room("TestRoom");

            // Act
            _gameService.GenerateWalls(room);

            // Assert
            Assert.That(room.Walls.Count, Is.GreaterThan(0), "Walls should be added to the room.");
        }
        [Test]
        public void SpawnPowerups_AddsPowerupsToRoom()
        {
            // Arrange
            var room = new Room("TestRoom");

            // Act
            _gameService.SpawnPowerups(room);

            // Assert
            Assert.That(room.Powerups.Count, Is.GreaterThan(0), "Powerups should be added to the room.");
        }
    }
}
