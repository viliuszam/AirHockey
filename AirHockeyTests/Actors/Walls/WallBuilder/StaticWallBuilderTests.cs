using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Walls.WallBuilder.Tests
{
    [TestFixture]
    public class StaticWallBuilderTests
    {
        [Test]
        public void StaticWallBuilder_Build_ReturnsCorrectWallType()
        {
            var builder = new StaticWallBuilder().SetType("Teleporting").SetDimensions(100, 50).SetPosition(10, 20);

            var wall = builder.Build();

            Assert.IsInstanceOf<TeleportingWall>(wall);
            Assert.AreEqual(100, wall.Width);
            Assert.AreEqual(50, wall.Height);
            Assert.AreEqual(10, wall.X);
            Assert.AreEqual(20, wall.Y);
        }
    }
}
