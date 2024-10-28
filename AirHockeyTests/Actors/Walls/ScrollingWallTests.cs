using AirHockey.Actors.Walls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Walls.Tests
{
    public class ScrollingWallTests
    {
        [Test]
        public void ScrollingWall_Movement_OscillatesPositionWithinBounds()
        {
            var wall = new ScrollingWall(1, 100, 50);

            int iterations = 2 * wall.GetIter();
            for (int i = 0; i < iterations; i++)
            {
                wall.Update();
            }

            Assert.Greater(wall.Y, 0);
            Assert.Less(wall.Y, 100);
        }

        [Test]
        public void ScrollingWall_Friction_UpdatesVelocity()
        {
            var wall = new ScrollingWall(1, 100, 50);

            wall.VelocityX = 10;
            wall.VelocityY = 20;
            wall.Update();
            wall.Update();

            Assert.Less(wall.VelocityX, 10);
            Assert.Less(wall.VelocityY, 20);
        }

        [Test]
        public void ScrollingWall_IterationCount_ChangesDirection()
        {
            var wall = new ScrollingWall(1, 100, 50);

            for (int i = 0; i < wall.GetIter(); i++)
            {
                wall.Update();
            }

            Assert.AreEqual("DOWN", wall.GetDirection());
        }
    }
}
