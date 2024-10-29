using AirHockey.Actors.Walls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockeyTests.Actors.Walls
{
    [TestFixture]
    public class DynamicWallFactoryTests
    {
        private DynamicWallFactory _dynamicWallFactory;

        [SetUp]
        public void Setup()
        {
            _dynamicWallFactory = new DynamicWallFactory();
        }

        [Test]
        public void CreateWall_CreatesWallWithGivenProperties()
        {
            int id = 1;
            float width = 10.0f;
            float height = 5.0f;
            string type = "Bouncy";
            float x = 2.0f;
            float y = 3.0f;

            Wall wall = _dynamicWallFactory.CreateWall(id, width, height, type, x, y);

            Assert.AreEqual(id, wall.Id, "Wall ID should match the provided ID.");
            Assert.AreEqual(width, wall.Width, "Wall width should match the provided width.");
            Assert.AreEqual(height, wall.Height, "Wall height should match the provided height.");
            Assert.AreEqual(x, wall.X, "Wall X coordinate should match the provided X.");
            Assert.AreEqual(y, wall.Y, "Wall Y coordinate should match the provided Y.");
        }
    }
}
