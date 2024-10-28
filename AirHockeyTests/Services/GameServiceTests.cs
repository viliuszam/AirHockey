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
        
    }
}