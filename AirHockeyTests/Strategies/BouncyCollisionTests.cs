using NUnit.Framework;
using AirHockey.Actors.Walls;
using AirHockey.Actors;

namespace AirHockey.Strategies.Tests
{
    [TestFixture]
    public class BouncyCollisionTests
    {
        private BouncyCollision _bouncyCollision;

        [SetUp]
        public void SetUp()
        {
            _bouncyCollision = new BouncyCollision();
        }

        [Test]
        public void ResolveCollision_WithNonWallEntity_NoCollisionResolution()
        {
            BouncyWall bouncyWall = new BouncyWall(1, 50, 50, moveable: false) { X = 50, Y = 50 };
            Puck puck = new Puck { X = 70, Y = 70, Radius = 5 };

            _bouncyCollision.ResolveCollision(bouncyWall, puck);

            Assert.AreNotEqual(70, puck.X);
            Assert.AreNotEqual(70, puck.Y);
        }

        [Test]
        public void ResolveCollision_WithNoOverlap_NoPositionChange()
        {
            BouncyWall bouncyWall = new BouncyWall(1, 50, 50, moveable: true) { X = 50, Y = 50 };
            StandardWall staticWall = new StandardWall(2, 50, 50, moveable: false) { X = 200, Y = 200 };

            _bouncyCollision.ResolveCollision(bouncyWall, staticWall);

            Assert.AreEqual(50, bouncyWall.X);
            Assert.AreEqual(50, bouncyWall.Y);
        }

        [Test]
        public void ResolveCollision_HorizontalCollision_DeltaXPositive_WithAcceleration()
        {
            BouncyWall bouncyWall = new BouncyWall(1, 50, 50, moveable: true) { X = 50, Y = 50 };
            StandardWall staticWall = new StandardWall(2, 50, 50, moveable: false) { X = 90, Y = 50, Acceleration = 1.0f };

            _bouncyCollision.ResolveCollision(bouncyWall, staticWall);

            Assert.AreNotEqual(90, staticWall.X);
            Assert.AreEqual(0, staticWall.VelocityX);
        }

        [Test]
        public void ResolveCollision_HorizontalCollision_DeltaXNegative_WithAcceleration()
        {
            BouncyWall bouncyWall = new BouncyWall(1, 50, 50, moveable: true) { X = 90, Y = 50 };
            StandardWall staticWall = new StandardWall(2, 50, 50, moveable: false) { X = 50, Y = 50, Acceleration = 1.0f };

            _bouncyCollision.ResolveCollision(bouncyWall, staticWall);

            Assert.AreNotEqual(50, staticWall.X);
            Assert.AreEqual(0, staticWall.VelocityX);
        }

        [Test]
        public void ResolveCollision_VerticalCollision_DeltaYPositive_WithAcceleration()
        {
            BouncyWall bouncyWall = new BouncyWall(1, 50, 50, moveable: true) { X = 50, Y = 90 };
            StandardWall staticWall = new StandardWall(2, 50, 50, moveable: false) { X = 50, Y = 50, Acceleration = 1.0f };

            _bouncyCollision.ResolveCollision(bouncyWall, staticWall);

            Assert.AreNotEqual(50, staticWall.Y);
            Assert.AreEqual(0, staticWall.VelocityY);
        }

        [Test]
        public void ResolveCollision_VerticalCollision_DeltaYNegative_WithAcceleration()
        {
            BouncyWall bouncyWall = new BouncyWall(1, 50, 50, moveable: true) { X = 50, Y = 10 };
            StandardWall staticWall = new StandardWall(2, 50, 50, moveable: false) { X = 50, Y = 50, Acceleration = 1.0f };

            _bouncyCollision.ResolveCollision(bouncyWall, staticWall);

            Assert.AreNotEqual(50, staticWall.Y);
            Assert.AreEqual(0, staticWall.VelocityY);
        }
        [Test]
        public void ResolveCollision_WhenGetMoveReturnsTrue_MovesBouncyWallAwayFromStandardWall()
        {
            int wallId = 1;
            float wallWidth = 20f;
            float wallHeight = 100f;
            StandardWall standardWall = new StandardWall(wallId, wallWidth, wallHeight, moveable: false)
            {
                X = 60f,
                Y = 50f
            };

            BouncyWall bouncyWall = new BouncyWall(2, 40f, 30f, true)
            {
                X = 50f,
                Y = 50f,
                VelocityX = 5f
            };

            _bouncyCollision.ResolveCollision(bouncyWall, standardWall);

            Assert.AreEqual(bouncyWall.X, 50f);
            Assert.AreEqual(bouncyWall.VelocityX * bouncyWall.GetBounce(), bouncyWall.VelocityX, 0.2);
        }


    }

}
