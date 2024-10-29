using NUnit.Framework;
using AirHockey.Actors.Walls;
using AirHockey.Actors;

namespace AirHockey.Strategies.Tests
{
    [TestFixture()]
    public class BouncyCollisionTests
    {
        private BouncyCollision _bouncyCollision;

        [SetUp]
        public void SetUp()
        {
            _bouncyCollision = new BouncyCollision();
        }

        [Test()]
        public void ResolveCollision_WithStaticBouncyWallAndStaticWall_NoMovement()
        {
            BouncyWall bouncyWall = new BouncyWall(1, 100, 200, moveable: false) { X = 50, Y = 50 };
            StandardWall staticWall = new StandardWall(2, 100, 200, moveable: false) { X = 60, Y = 50 };

            _bouncyCollision.ResolveCollision(bouncyWall, staticWall);

            Assert.AreEqual(50, bouncyWall.X);
            Assert.AreEqual(50, bouncyWall.Y);
            Assert.AreEqual(60, staticWall.X);
            Assert.AreEqual(50, staticWall.Y);
            Assert.AreEqual(0, bouncyWall.VelocityX);
            Assert.AreEqual(0, bouncyWall.VelocityY);
        }

        [Test()]
        public void ResolveCollision_WithMovingPuckAndStaticBouncyWall_BounceBack()
        {
            BouncyWall bouncyWall = new BouncyWall(1, 100, 200, moveable: false) { X = 50, Y = 50 };
            Puck puck = new Puck { X = 60, Y = 50, VelocityX = -5, VelocityY = 0, Radius = 10 };

            _bouncyCollision.ResolveCollision(bouncyWall, puck);

            Assert.AreNotEqual(60, puck.X);
            Assert.AreNotEqual(5 * bouncyWall.GetBounce(), puck.VelocityX); 
            Assert.AreEqual(0, puck.VelocityY); 
        }

        [Test()]
        public void ResolveCollision_WithMovingPuckAndMovableBouncyWall_BothMove()
        {
            BouncyWall bouncyWall = new BouncyWall(1, 100, 200, moveable: true) { X = 50, Y = 50, VelocityX = 0, VelocityY = 0 };
            Puck puck = new Puck { X = 55, Y = 50, VelocityX = -5, VelocityY = 0, Radius = 10, Mass = 1 };

            _bouncyCollision.ResolveCollision(bouncyWall, puck);

            Assert.AreEqual(0, bouncyWall.VelocityX); 
            Assert.AreNotEqual(-5, puck.VelocityX); 
            Assert.AreEqual(0, puck.VelocityY); 
        }
    }
}
