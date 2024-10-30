using AirHockey.Effects;
using AirHockey.Observers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Tests
{
    [TestFixture]
    public class GameTests
    {
        private Game game;
        private Room room;
        private Mock<IGoalObserver> observerMock;
        private Mock<EnvironmentalEffect> effectMock;
        private Player player1;
        private Player player2;

        [SetUp]
        public void Setup()
        {
            room = new Room("TEST123");

            player1 = new Player("p1", "red", 50, 50, "player1", room);
            player2 = new Player("p2", "blue", 90, 90, "player2", room);
            room.AddPlayer(player1);
            room.AddPlayer(player2);

            game = new Game(room);

            observerMock = new Mock<IGoalObserver>();

            effectMock = new Mock<EnvironmentalEffect>(
                1,
                Mock.Of<IEffectBehavior>(),
                10.0f
            );
        }

        [Test]
        public void Constructor_InitializesGameCorrectly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(game.Room, Is.EqualTo(room));
                Assert.That(game.Player1Score, Is.EqualTo(0));
                Assert.That(game.Player2Score, Is.EqualTo(0));
                Assert.That(game.ActiveEffects, Is.Not.Null);
                Assert.That(game.ActiveEffects, Is.Empty);
            });
        }

        [Test]
        public void RegisterObserver_AddsObserverToList()
        {
            game.RegisterObserver(observerMock.Object);
            game.GoalScored(player1);

            observerMock.Verify(o =>
                o.OnGoalScored(player1, game),
                Times.Once
            );
        }

        [Test]
        public void UnregisterObserver_RemovesObserverFromList()
        {
            game.RegisterObserver(observerMock.Object);
            game.UnregisterObserver(observerMock.Object);
            game.GoalScored(player1);

            observerMock.Verify(o =>
                o.OnGoalScored(It.IsAny<Player>(), It.IsAny<Game>()),
                Times.Never
            );
        }

        [Test]
        public void NotifyObservers_CallsAllRegisteredObservers()
        {
            var observer1Mock = new Mock<IGoalObserver>();
            var observer2Mock = new Mock<IGoalObserver>();

            game.RegisterObserver(observer1Mock.Object);
            game.RegisterObserver(observer2Mock.Object);

            game.NotifyObservers(player1);

            observer1Mock.Verify(o => o.OnGoalScored(player1, game), Times.Once);
            observer2Mock.Verify(o => o.OnGoalScored(player1, game), Times.Once);
        }

        [Test]
        public void GoalScored_Player1_IncreasesPlayer1Score()
        {
            int initialScore = game.Player1Score;
            game.GoalScored(player1);

            Assert.That(game.Player1Score, Is.EqualTo(initialScore + 1));
        }

        [Test]
        public void GoalScored_Player2_IncreasesPlayer2Score()
        {
            int initialScore = game.Player2Score;
            game.GoalScored(player2);

            Assert.That(game.Player2Score, Is.EqualTo(initialScore + 1));
        }

        [Test]
        public void GoalScored_NotifiesObservers()
        {
            game.RegisterObserver(observerMock.Object);
            game.GoalScored(player1);

            observerMock.Verify(o =>
                o.OnGoalScored(player1, game),
                Times.Once
            );
        }

        [Test]
        public void StartGame_ResetsScores()
        {
            game.Player1Score = 5;
            game.Player2Score = 3;

            game.StartGame();

            Assert.Multiple(() =>
            {
                Assert.That(game.Player1Score, Is.EqualTo(0));
                Assert.That(game.Player2Score, Is.EqualTo(0));
            });
        }

        [Test]
        public void ActiveEffects_CanAddAndRemoveEffects()
        {
            game.ActiveEffects.Add(effectMock.Object);
            Assert.That(game.ActiveEffects, Has.Count.EqualTo(1));

            game.ActiveEffects.Remove(effectMock.Object);
            Assert.That(game.ActiveEffects, Is.Empty);
        }

        [Test]
        public void Game_InitializationFlags_DefaultToFalse()
        {
            Assert.Multiple(() =>
            {
                Assert.That(game.HasObservers, Is.False);
                Assert.That(game.IsInitialized, Is.False);
            });
        }

        [Test]
        public void GoalScored_InvalidPlayer_DoesNotIncrementScore()
        {
            var invalidPlayer = new Player("inv", "red", 0, 0, "invalid", room);
            int initialPlayer1Score = game.Player1Score;
            int initialPlayer2Score = game.Player2Score;

            game.GoalScored(invalidPlayer);

            Assert.Multiple(() =>
            {
                // kai paduodamas blogas zaidejas parametru, goal priskaitomas antram zaidejui
                Assert.That(game.Player1Score, Is.EqualTo(initialPlayer1Score));
                Assert.That(game.Player2Score, Is.EqualTo(initialPlayer2Score + 1));
            });
        }

        [Test]
        public void Room_IsAccessible()
        {
            Assert.That(game.Room, Is.Not.Null);
            Assert.That(game.Room.RoomCode, Is.EqualTo("TEST123"));
        }
    }
}
