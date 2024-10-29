using Moq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Walls.Tests
{
    public class TestWall : Wall
    {
        public TestWall(int id, float width, float height) : base(id, width, height) { }
        public TestWall() : base() { }

        public override void Update()
        {}
    }

    [TestFixture]
    public class WallTests
    {
        private TestWall wall;
        private Mock<Entity> mockEntity;

        [SetUp]
        public void Setup()
        {
            wall = new TestWall(1, 100f, 50f)
            {
                X = 0f,
                Y = 0f
            };

            mockEntity = new Mock<Entity>();
        }

        [Test]
        public void Constructor_WithParameters_SetsProperties()
        {
            const int expectedId = 2;
            const float expectedWidth = 150f;
            const float expectedHeight = 75f;

            var wall = new TestWall(expectedId, expectedWidth, expectedHeight);

            Assert.Multiple(() =>
            {
                Assert.That(wall.Id, Is.EqualTo(expectedId));
                Assert.That(wall.Width, Is.EqualTo(expectedWidth));
                Assert.That(wall.Height, Is.EqualTo(expectedHeight));
            });
        }

        [Test]
        public void IsColliding_WithWall_DetectsCollision()
        {
            var otherWall = new TestWall(2, 50f, 50f)
            {
                X = 50f,
                Y = 25f
            };

            bool isColliding = wall.IsColliding(otherWall);

            Assert.That(isColliding, Is.True);
        }

        [Test]
        public void IsColliding_WithWall_NoCollision()
        {
            var otherWall = new TestWall(2, 50f, 50f)
            {
                X = 200f,
                Y = 200f
            };

            bool isColliding = wall.IsColliding(otherWall);

            Assert.That(isColliding, Is.False);
        }

        [Test]
        public void IsColliding_WithRectangle_DetectsCollision()
        {
            var exclusionZone = new Rectangle(50, 25, 50, 50);

            bool isColliding = wall.IsColliding(exclusionZone);

            Assert.That(isColliding, Is.True);
        }

        [Test]
        public void IsColliding_WithRectangle_NoCollision()
        {
            var exclusionZone = new Rectangle(200, 200, 50, 50);

            bool isColliding = wall.IsColliding(exclusionZone);

            Assert.That(isColliding, Is.False);
        }

        [Test]
        public void ConstrainToBounds_ExceedsMinX_ConstrainsPosition()
        {
            wall.X = -10f;
            wall.VelocityX = -5f;

            wall.ConstrainToBounds(0f, 0f, 800f, 600f);

            Assert.Multiple(() =>
            {
                Assert.That(wall.X, Is.EqualTo(0f));
                Assert.That(wall.VelocityX, Is.EqualTo(2.5f));  // -(-5 * 0.5f)
            });
        }

        [Test]
        public void ConstrainToBounds_ExceedsMaxX_ConstrainsPosition()
        {
            wall.X = 750f;
            wall.VelocityX = 5f;

            wall.ConstrainToBounds(0f, 0f, 800f, 600f);

            Assert.Multiple(() =>
            {
                Assert.That(wall.X, Is.EqualTo(700f));  // maxX (800) - Width (100)
                Assert.That(wall.VelocityX, Is.EqualTo(-2.5f));  // -(5 * 0.5f)
            });
        }

        [Test]
        public void ConstrainToBounds_ExceedsMinY_ConstrainsPosition()
        {
            wall.Y = -10f;
            wall.VelocityY = -5f;

            wall.ConstrainToBounds(0f, 0f, 800f, 600f);

            Assert.Multiple(() =>
            {
                Assert.That(wall.Y, Is.EqualTo(0f));
                Assert.That(wall.VelocityY, Is.EqualTo(2.5f));  // -(-5 * 0.5f)
            });
        }

        [Test]
        public void ConstrainToBounds_ExceedsMaxY_ConstrainsPosition()
        {
            wall.Y = 575f;
            wall.VelocityY = 5f;

            wall.ConstrainToBounds(0f, 0f, 800f, 600f);

            Assert.Multiple(() =>
            {
                Assert.That(wall.Y, Is.EqualTo(550f));  // maxY (600) - Height (50)
                Assert.That(wall.VelocityY, Is.EqualTo(-2.5f));  // -(5 * 0.5f)
            });
        }

        [Test]
        public void IsColliding_WithEntity_DetectsCollision()
        {
            Puck puck = new Puck();
            puck.X = 0;
            puck.Y = 0;

            wall.X = 0f;
            wall.Y = 0f;
            wall.Width = 50f;
            wall.Height = 50f;

            // Act
            bool isColliding = wall.IsColliding(puck);

            // Assert
            Assert.That(isColliding, Is.True, "Expected a collision when entity is within wall bounds.");
        }

        [Test]
        public void IsColliding_WithEntity_NoCollision()
        {
            Puck puck = new Puck();
            puck.X = 100;
            puck.Y = 100;

            wall.X = 0f;
            wall.Y = 0f;
            wall.Width = 50f;
            wall.Height = 50f;

            // Act
            bool isColliding = wall.IsColliding(puck);

            // Assert
            Assert.That(isColliding, Is.False, "Expected no collision when entity is outside wall bounds.");
        }
    }
}
