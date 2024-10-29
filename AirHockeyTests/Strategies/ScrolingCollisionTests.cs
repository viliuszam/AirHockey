using NUnit.Framework;
using AirHockey.Actors;
using AirHockey.Actors.Walls;
using AirHockey.Strategies;

namespace AirHockey.Strategies.Tests
{
    [TestFixture()]
    public class ScrollingCollisionTests
    {
        private ScrolingCollision _scrollingCollision;

        [SetUp]
        public void SetUp()
        {
            _scrollingCollision = new ScrolingCollision();
        }

        [Test()]
        public void ResolveCollision_WithScrollingWallAndStaticWall_CorrectPositionAdjustment()
        {
            ScrollingWall scrollingWall = new ScrollingWall(1, 100, 200) { X = 50, Y = 50 };
            StandardWall staticWall = new StandardWall(2, 100, 200, moveable: false) { X = 60, Y = 50 }; // Added moveable parameter

            _scrollingCollision.ResolveCollision(scrollingWall, staticWall);

            Assert.AreNotEqual(50, scrollingWall.X, "Scrolling wall should have adjusted its position due to collision.");
            Assert.AreEqual(60, staticWall.X, "Static wall's position should remain the same.");
        }

        [Test()]
        public void ResolveCollision_WithScrollingWallAndMovingPuck_CorrectVelocityAdjustment()
        {
            ScrollingWall scrollingWall = new ScrollingWall(1, 100, 200) { X = 50, Y = 50, VelocityY = 1 };
            Puck puck = new Puck { X = 55, Y = 50, VelocityX = -5, VelocityY = 0, Radius = 10 };

            _scrollingCollision.ResolveCollision(scrollingWall, puck);

            Assert.AreNotEqual(-5, puck.VelocityX, "Puck's horizontal velocity should be adjusted after collision.");
            Assert.AreEqual(0, puck.VelocityY, "Puck's vertical velocity should be affected by the scrolling wall.");
        }

        [Test()]
        public void ResolveCollision_NoOverlap_NoPositionChange()
        {
            ScrollingWall scrollingWall = new ScrollingWall(1, 100, 200) { X = 50, Y = 50 };
            StandardWall staticWall = new StandardWall(2, 100, 200, moveable: false) { X = 200, Y = 50 }; // No overlap

            _scrollingCollision.ResolveCollision(scrollingWall, staticWall);

            Assert.AreEqual(50, scrollingWall.X, "Scrolling wall's position should remain unchanged.");
            Assert.AreEqual(200, staticWall.X, "Static wall's position should remain unchanged.");
        }
    }
}
