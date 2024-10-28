using AirHockey.Actors.Walls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Walls.Tests
{
    public class UndoWallTests
    {
        [Test]
        public void UndoWall_Deactivate_ReactivatesAfterIterationCount()
        {
            var wall = new UndoWall(1, 100, 50);

            wall.setInactive();
            for (int i = 0; i < wall.GetIter() + 1; i++)
            {
                wall.Update();
            }

            Assert.IsTrue(wall.isActive());
        }

        [Test]
        public void UndoWall_Update_DoesNotChangePositionWhenInactive()
        {
            var wall = new UndoWall(1, 100, 50);

            var originalX = wall.X;
            var originalY = wall.Y;
            wall.setInactive();
            wall.Update();

            Assert.AreEqual(originalX, wall.X);
            Assert.AreEqual(originalY, wall.Y);
        }
    }
}
