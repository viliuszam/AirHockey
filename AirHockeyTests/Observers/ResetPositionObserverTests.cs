using NUnit.Framework;
using AirHockey.Actors;
using AirHockey.Observers;

namespace AirHockey.Observers.Tests
{
    [TestFixture()]
    public class ResetPositionObserverTests
    {
        private Game game;
        private ResetPositionObserver observer;

        [SetUp]
        public void SetUp()
        {
            // Set up a Room with a roomCode
            var roomCode = "TestRoom";
            var room = new Room(roomCode);
            var player1 = new Player("1", "Red", 0, 0, "Player1", room);
            var player2 = new Player("2", "Blue", 0, 0, "Player2", room);
            room.Players.Add(player1);
            room.Players.Add(player2);

            game = new Game(room);
            observer = new ResetPositionObserver();
        }

        [Test()]
        public void OnGoalScored_ShouldResetPuckAndPlayersPositionsAndVelocities()
        {
            // Arrange: Set initial positions and velocities
            game.Room.Puck.X = 100;
            game.Room.Puck.Y = 200;
            game.Room.Puck.VelocityX = 5;
            game.Room.Puck.VelocityY = 5;

            game.Room.Players[0].X = 50;
            game.Room.Players[0].Y = 50;
            game.Room.Players[0].VelocityX = 3;
            game.Room.Players[0].VelocityY = 3;

            game.Room.Players[1].X = 100;
            game.Room.Players[1].Y = 100;
            game.Room.Players[1].VelocityX = -3;
            game.Room.Players[1].VelocityY = -3;

            // Act: Simulate a goal being scored
            observer.OnGoalScored(game.Room.Players[0], game);

            // Assert: Check the puck's position and velocity
            Assert.AreEqual(427.0f, game.Room.Puck.X);
            Assert.AreEqual(270.0f, game.Room.Puck.Y);
            Assert.AreEqual(0, game.Room.Puck.VelocityX);
            Assert.AreEqual(0, game.Room.Puck.VelocityY);

            // Assert: Check Player 1's position and velocity
            Assert.AreEqual(227, game.Room.Players[0].X);
            Assert.AreEqual(260, game.Room.Players[0].Y);
            Assert.AreEqual(0, game.Room.Players[0].VelocityX);
            Assert.AreEqual(0, game.Room.Players[0].VelocityY);

            // Assert: Check Player 2's position and velocity
            Assert.AreEqual(633, game.Room.Players[1].X);
            Assert.AreEqual(260, game.Room.Players[1].Y);
            Assert.AreEqual(0, game.Room.Players[1].VelocityX);
            Assert.AreEqual(0, game.Room.Players[1].VelocityY);
        }
    }
}
