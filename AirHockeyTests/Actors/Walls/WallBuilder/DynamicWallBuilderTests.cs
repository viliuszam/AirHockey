using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Walls.WallBuilder.Tests
{
    [TestFixture]
    public class DynamicWallBuilderTests
    {
        [Test]
        public void DynamicWallBuilder_Build_ReturnsCorrectWallType_Bouncy()
        {
            var builder = new DynamicWallBuilder();
            builder.SetId(1)
                   .SetType("Bouncy")
                   .SetDimensions(100, 50)
                   .SetPosition(10, 20)
                   .SetVelocity(5, 10)
                   .SetAcceleration()
                   .SetMass();

            var wall = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<BouncyWall>(wall);
                Assert.AreEqual(1, wall.Id);
                Assert.AreEqual(100, wall.Width);
                Assert.AreEqual(50, wall.Height);
                Assert.AreEqual(10, wall.X);
                Assert.AreEqual(20, wall.Y);
                Assert.AreEqual(5, wall.VelocityX);
                Assert.AreEqual(10, wall.VelocityY);
                Assert.AreEqual(0.95f, wall.Acceleration);
                Assert.AreEqual(0.1f, wall.Mass);
            });
        }

        [Test]
        public void DynamicWallBuilder_Build_ReturnsCorrectWallType_Scrolling()
        {
            var builder = new DynamicWallBuilder();
            builder.SetId(2)
                   .SetType("Scrolling")
                   .SetDimensions(150, 75)
                   .SetPosition(30, 40)
                   .SetVelocity(15, 20)
                   .SetAcceleration()
                   .SetMass();

            var wall = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<ScrollingWall>(wall);
                Assert.AreEqual(2, wall.Id);
                Assert.AreEqual(150, wall.Width);
                Assert.AreEqual(75, wall.Height);
                Assert.AreEqual(30, wall.X);
                Assert.AreEqual(40, wall.Y);
                Assert.AreEqual(15, wall.VelocityX);
                Assert.AreEqual(20, wall.VelocityY);
                Assert.AreEqual(0.75f, wall.Acceleration);
                Assert.AreEqual(1.0f, wall.Mass);
            });
        }

        [Test]
        public void DynamicWallBuilder_SetInvalidType_ThrowsArgumentException()
        {
            var builder = new DynamicWallBuilder();

            var exception = Assert.Throws<ArgumentException>(() => builder.SetType("InvalidType"));

            Assert.AreEqual("Invalid wall type: InvalidType.", exception.Message);
        }

        [Test]
        public void DynamicWallBuilder_NoTypeBuild()
        {
            var builder = new DynamicWallBuilder();
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Test]
        public void DynamicWallBuilder_SetId_SetsWallId()
        {
            var builder = new DynamicWallBuilder().SetId(42);

            var wall = builder.SetType("Bouncy").SetDimensions(100, 50).Build();

            Assert.AreEqual(42, wall.Id);
        }

        [Test]
        public void DynamicWallBuilder_Build_WithInvalidWallType_ThrowsArgumentException()
        {
            var builder = new DynamicWallBuilder().SetDimensions(100, 50);

            Assert.Throws<ArgumentException>(() => builder.SetType("InvalidType"));
        }
    }
}
