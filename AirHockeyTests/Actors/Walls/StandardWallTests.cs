using AirHockey.Actors.Walls;
using AirHockey.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Walls.Tests
{
    public class StandardWallTests
    {
        [Test]
        public void StandardWall_Update_DoesNotChangePosition()
        {
            var wall = new StandardWall(1, 100, 50, true);

            var originalX = wall.X;
            var originalY = wall.Y;
            wall.Update();

            Assert.AreEqual(originalX, wall.X);
            Assert.AreEqual(originalY, wall.Y);
        }
    }
}
