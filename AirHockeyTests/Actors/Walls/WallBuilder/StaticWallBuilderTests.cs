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
        public void StaticWallBuilder_BuildTeleportingWall_ReturnsCorrectWallType()
        {
            var builder = new StaticWallBuilder()
                .SetType("Teleporting")
                .SetDimensions(100, 50)
                .SetPosition(10, 20);

            var wall = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<TeleportingWall>(wall);
                Assert.AreEqual(100, wall.Width);
                Assert.AreEqual(50, wall.Height);
                Assert.AreEqual(10, wall.X);
                Assert.AreEqual(20, wall.Y);
            });
        }

        [Test]
        public void StaticWallBuilder_BuildBouncyWall_ReturnsCorrectWallType()
        {
            var builder = new StaticWallBuilder()
                .SetType("Bouncy")
                .SetDimensions(200, 100)
                .SetPosition(30, 40)
                .SetMass();

            var wall = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<BouncyWall>(wall);
                Assert.AreEqual(200, wall.Width);
                Assert.AreEqual(100, wall.Height);
                Assert.AreEqual(30, wall.X);
                Assert.AreEqual(40, wall.Y);
                Assert.AreEqual(500f, wall.Mass);
            });
        }

        [Test]
        public void StaticWallBuilder_BuildStandardWall_ReturnsCorrectWallType()
        {
            var builder = new StaticWallBuilder()
                .SetType("Standard")
                .SetDimensions(150, 75)
                .SetPosition(50, 60)
                .SetMass();

            var wall = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<StandardWall>(wall);
                Assert.AreEqual(150, wall.Width);
                Assert.AreEqual(75, wall.Height);
                Assert.AreEqual(50, wall.X);
                Assert.AreEqual(60, wall.Y);
                Assert.AreEqual(500f, wall.Mass);
            });
        }

        [Test]
        public void StaticWallBuilder_BuildUndoWall_ReturnsCorrectWallType()
        {
            var builder = new StaticWallBuilder()
                .SetType("Undo")
                .SetDimensions(120, 60)
                .SetPosition(15, 25)
                .SetMass();

            var wall = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<UndoWall>(wall);
                Assert.AreEqual(120, wall.Width);
                Assert.AreEqual(60, wall.Height);
                Assert.AreEqual(15, wall.X);
                Assert.AreEqual(25, wall.Y);
                Assert.AreEqual(0f, wall.Mass);
            });
        }

        [Test]
        public void StaticWallBuilder_BuildQSWall_ReturnsCorrectWallType()
        {
            var builder = new StaticWallBuilder()
                .SetType("QuickSand")
                .SetDimensions(120, 60)
                .SetPosition(15, 25)
                .SetMass();

            var wall = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<QuickSandWall>(wall);
                Assert.AreEqual(120, wall.Width);
                Assert.AreEqual(60, wall.Height);
                Assert.AreEqual(15, wall.X);
                Assert.AreEqual(25, wall.Y);
                Assert.AreEqual(0f, wall.Mass);
            });
        }

        [Test]
        public void StaticWallBuilder_NoTypeBuild_ThrowsArgumentException()
        {
            var builder = new StaticWallBuilder();
            Assert.Throws<ArgumentException>(() => builder.Build());

        }

            [Test]
        public void StaticWallBuilder_SetInvalidType_ThrowsArgumentException()
        {
            var builder = new StaticWallBuilder();

            var exception = Assert.Throws<ArgumentException>(() => builder.SetType("InvalidType"));

            Assert.AreEqual("Invalid wall type: InvalidType.", exception.Message);
        }
    }
}
