using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Walls.Tests
{
    public class BouncyWallTests
    {

        [Test]
        public void BouncyWall_Movement_UpdatesPositionAndVelocity()
        {
            var wall = new BouncyWall(1, 100, 50, true);
            wall.X = 0;
            wall.Y = 0;

            wall.VelocityX = 5;
            wall.VelocityY = 10;
            wall.Update();

            Assert.AreNotEqual(5, wall.VelocityX);
            Assert.AreNotEqual(10, wall.VelocityY);
            Assert.AreNotEqual(0, wall.X);
            Assert.AreNotEqual(0, wall.Y);
        }

        [Test]
        public void BouncyWall_Friction_UpdatesVelocity()
        { 
            var wall = new BouncyWall(1, 100, 50, true);

            wall.VelocityX = 10;
            wall.VelocityY = 20;
            wall.Update();
            wall.Update();

            Assert.Less(wall.VelocityX, 10);
            Assert.Less(wall.VelocityY, 20);
        }
    }
}
