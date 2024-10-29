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
            StandardWall staticWall = new StandardWall(2, 100, 200, moveable: false) { X = 60, Y = 50 }; 

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
        [Test()]
        public void ResolveCollision_WithScrollingWallAndStaticWall_VerticalCollision()
        {
            ScrollingWall scrollingWall = new ScrollingWall(1, 100, 200) { X = 50, Y = 100, VelocityY = 1 }; 
            StandardWall staticWall = new StandardWall(2, 100, 200, moveable: false) { X = 50, Y = 80 }; 

            _scrollingCollision.ResolveCollision(scrollingWall, staticWall);

            Assert.AreEqual(100, scrollingWall.Y, "Scrolling wall should have adjusted its position due to vertical collision.");
            Assert.AreEqual(50, staticWall.X, "Static wall's position should remain the same.");
            Assert.AreEqual(80, staticWall.Y, "Static wall's Y position should remain unchanged.");
        }
        [Test()]
        public void ResolveCollision_WithScrollingWallAndStaticWall_ZeroAcceleration()
        {
            ScrollingWall scrollingWall = new ScrollingWall(1, 100, 200) { X = 50, Y = 50, VelocityY = 1 };
            StandardWall staticWall = new StandardWall(2, 100, 200, moveable: false)
            {
                X = 50,
                Y = 100, 
                Acceleration = 0.1f 
            };

            _scrollingCollision.ResolveCollision(scrollingWall, staticWall);

            Assert.AreEqual(50, scrollingWall.Y, "Scrolling wall should have adjusted its position due to collision with a static wall with zero acceleration.");
            Assert.AreEqual(0, staticWall.X, "Static wall's position should remain the same.");
            Assert.AreEqual(100, staticWall.Y, "Static wall's Y position should remain unchanged.");
        }
        [Test()]
        public void ResolveCollision_WithScrollingWallAndStaticWall_ZeroAcceleration2()
        {
            ScrollingWall scrollingWall = new ScrollingWall(1, 100, 200)
            {
                X = 50,
                Y = 50,
                VelocityY = 1
            };
            StandardWall staticWall = new StandardWall(2, 100, 200, moveable: false)
            {
                X = 150, 
                Y = 100,  
                Acceleration = 0.1f 
            };

            _scrollingCollision.ResolveCollision(scrollingWall, staticWall);

            Assert.AreEqual(50, scrollingWall.X, "Scrolling wall should not have adjusted its position due to collision with a static wall.");
            Assert.AreEqual(50, scrollingWall.Y, "Scrolling wall should not have adjusted its position due to collision with a static wall.");
            Assert.AreEqual(150, staticWall.X, "Static wall's position should remain the same.");
            Assert.AreEqual(100, staticWall.Y, "Static wall's Y position should remain unchanged.");
        }
        [Test()]
        public void ResolveCollision_WithScrollingWallAndPuck_CorrectVelocityAdjustment()
        {
            ScrollingWall scrollingWall = new ScrollingWall(1, 100, 200)
            {
                X = 50,
                Y = 50,
                VelocityY = 1
            };
            Puck puck = new Puck
            {
                X = 55,
                Y = 50,  
                VelocityX = -5,  
                VelocityY = 0,
                Radius = 10
            };

            _scrollingCollision.ResolveCollision(scrollingWall, puck);

            Assert.AreNotEqual(-5, puck.VelocityX, "Puck's horizontal velocity should be adjusted after collision with scrolling wall.");
            Assert.AreEqual(0, puck.VelocityY, "Puck's vertical velocity should remain unchanged if the scrolling wall does not move vertically.");

            Assert.AreEqual(0, puck.X, "Puck's position should be adjusted due to collision with scrolling wall.");
        }
        [Test()]
        public void ResolveCollision_WithScrollingWallAndStaticWall_OverlapXGreaterThanOverlapY()
        {
            ScrollingWall scrollingWall = new ScrollingWall(1, 200, 100)
            {
                X = 100, 
                Y = 50,
                VelocityY = 1
            };
            StandardWall staticWall = new StandardWall(2, 200, 100, moveable: false)
            {
                X = 120, 
                Y = 80,   
                Acceleration = 0.1f 
            };

            _scrollingCollision.ResolveCollision(scrollingWall, staticWall);

            Assert.AreEqual(100, scrollingWall.X, "Scrolling wall should not have adjusted its position due to collision with a static wall.");
            Assert.AreEqual(15, scrollingWall.Y, "Scrolling wall should not have adjusted its position due to collision with a static wall.");
            Assert.AreEqual(120, staticWall.X, "Static wall's position should remain the same.");
            Assert.AreEqual(115, staticWall.Y, "Static wall's Y position should remain unchanged.");
        }
        [Test()]
        public void ResolveCollision_WithScrollingWallAndStaticWall_OverlapXGreaterThanOverlapY2()
        {
            ScrollingWall scrollingWall = new ScrollingWall(1, 100, 220)
            {
                X = 100,
                Y = 50,
                VelocityY = 1
            };
            StandardWall staticWall = new StandardWall(2, 110, 230, moveable: false)
            {
                X = 100,
                Y = 80,
                Acceleration = 0.1f
            };

            _scrollingCollision.ResolveCollision(scrollingWall, staticWall);

            Assert.AreEqual(50, scrollingWall.X, "Scrolling wall should not have adjusted its position due to collision with a static wall.");
            Assert.AreEqual(50, scrollingWall.Y, "Scrolling wall should not have adjusted its position due to collision with a static wall.");
            Assert.AreEqual(150, staticWall.X, "Static wall's position should remain the same.");
            Assert.AreEqual(80, staticWall.Y, "Static wall's Y position should remain unchanged.");
        }
        [Test()]
        public void ResolveCollision_WithScrollingWallAndStaticWall_EqualOverlaps()
        {
            // Arrange
            ScrollingWall scrollingWall = new ScrollingWall(1, 20, 20) 
            {
                X = 100,  
                Y = 50,  
                VelocityY = 1
            };

            StandardWall staticWall = new StandardWall(2, 20, 20, moveable: false) 
            {
                X = 90,  
                Y = 40,  
                Acceleration = 0.1f
            };

            _scrollingCollision.ResolveCollision(scrollingWall, staticWall);

            Assert.AreEqual(100, scrollingWall.X, "Scrolling wall should not have adjusted its position due to collision with a static wall.");
            Assert.AreEqual(55, scrollingWall.Y, "Scrolling wall should not have adjusted its position due to collision with a static wall.");
            Assert.AreEqual(90, staticWall.X, "Static wall's position should remain the same.");
            Assert.AreEqual(35, staticWall.Y, "Static wall's Y position should remain unchanged.");
        }

    }
}
