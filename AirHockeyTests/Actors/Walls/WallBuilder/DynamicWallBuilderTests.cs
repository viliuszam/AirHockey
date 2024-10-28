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
        public void DynamicWallBuilder_Build_ReturnsCorrectWallType()
        {
            var builder = new DynamicWallBuilder();
            builder.SetType("Bouncy").SetDimensions(100, 50).SetPosition(10, 20).SetVelocity(5, 10);

            var wall = builder.Build();

            Assert.IsInstanceOf<BouncyWall>(wall);
            Assert.AreEqual(100, wall.Width);
            Assert.AreEqual(50, wall.Height);
            Assert.AreEqual(10, wall.X);
            Assert.AreEqual(20, wall.Y);
            Assert.AreEqual(5, wall.VelocityX);
            Assert.AreEqual(10, wall.VelocityY);
        }
    }
}
