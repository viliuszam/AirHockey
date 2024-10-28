using AirHockey.Actors.Walls;
using AirHockey.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Walls.Tests
{
    public class QuickSandWallTests
    {
        [Test]
        public void QuickSandWall_Update_DoesNotChangePosition()
        {
            var wall = new QuickSandWall(1, 100, 50);

            var originalX = wall.X;
            var originalY = wall.Y;
            wall.Update();

            Assert.AreEqual(originalX, wall.X);
            Assert.AreEqual(originalY, wall.Y);
        }
    }
}
