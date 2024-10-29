using NUnit.Framework;
using AirHockey.Actors;
using AirHockey.Actors.Walls;
using AirHockey.Strategies;

namespace AirHockey.Strategies.Tests
{
    [TestFixture()]
    public class WallCollisionTests
    {
        private WallCollision _wallCollision;

        [SetUp]
        public void SetUp()
        {
            _wallCollision = new WallCollision();
        }

        [Test()]
        public void ResolveCollisionTest()
        {
            Wall wall = new StandardWall(1, 100, 200, false) { X = 50, Y = 100, Radius = 0 };
            Puck puck = new Puck { X = 70, Y = 150, VelocityX = 10, VelocityY = 0, Radius = 15f };

            _wallCollision.ResolveCollision(wall, puck);

            Assert.AreEqual(0.0f, puck.X, 0.01f);
            Assert.AreEqual(0.0f, puck.Y, 0.01f);

            Assert.AreEqual(puck.VelocityX, 0.0f);
            Assert.AreEqual(0.0f, puck.VelocityY);
        }
    }
}
