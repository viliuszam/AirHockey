using NUnit.Framework;
using AirHockey.Actors;
using AirHockey.Actors.Walls;

namespace AirHockey.Strategies.Tests
{
    [TestFixture()]
    public class QuickCollisionTests
    {
        private QuickCollision _quickCollision;

        [SetUp]
        public void SetUp()
        {
            _quickCollision = new QuickCollision();
        }

        [Test()]
        public void ResolveCollision_WithPuckAndQuickSandWall_VelocityReduced()
        {
            QuickSandWall quickSandWall = new QuickSandWall(1, 100, 200) { X = 50, Y = 50 };
            Puck puck = new Puck { X = 45, Y = 50, VelocityX = 2, VelocityY = 1, Radius = 10 };
            float expectedVelocityX = puck.VelocityX * quickSandWall.GetSlowFactor(); 
            float expectedVelocityY = puck.VelocityY * quickSandWall.GetSlowFactor(); 

            _quickCollision.ResolveCollision(quickSandWall, puck);

            Assert.AreEqual(expectedVelocityX, puck.VelocityX, 0.01f, $"VelocityX was not reduced correctly = {puck.VelocityX}");
            Assert.AreEqual(expectedVelocityY, puck.VelocityY, 0.01f, "VelocityY was not reduced correctly");
        }

        [Test()]
        public void ResolveCollision_WithPuckAndQuickSandWall_ZeroVelocity()
        {
            QuickSandWall quickSandWall = new QuickSandWall(1, 100, 200) { X = 50, Y = 50 };
            Puck puck = new Puck { X = 45, Y = 50, VelocityX = 0, VelocityY = 0, Radius = 10 };

            _quickCollision.ResolveCollision(quickSandWall, puck);

            puck.Update();

            Assert.AreEqual(0, puck.VelocityX);
            Assert.AreEqual(0, puck.VelocityY);
        }

        [Test()]
        public void ResolveCollision_WithFastPuck_VelocityCappedAfterCollision()
        {
            QuickSandWall quickSandWall = new QuickSandWall(1, 100, 200) { X = 50, Y = 50 };
            Puck puck = new Puck { X = 45, Y = 50, VelocityX = 20, VelocityY = 20, Radius = 10 }; 

            _quickCollision.ResolveCollision(quickSandWall, puck);

            puck.Update();

            Assert.LessOrEqual(Math.Sqrt(puck.VelocityX * puck.VelocityX + puck.VelocityY * puck.VelocityY), puck.MaxSpeed, "Puck velocity exceeds MaxSpeed after collision");
        }
    }
}
