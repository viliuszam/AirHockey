using NUnit.Framework;
using AirHockey.Strategies;
using AirHockey.Actors;

namespace AirHockey.Strategies.Tests
{
    [TestFixture()]
    public class BaseCollisionTests
    {

        private class TestEntity : Entity
        {
            public override void Update()
            {
 
            }
        }

        [Test()]
        public void ResolveCollisionTest()
        {
            var entityA = new TestEntity
            {
                X = 0,
                Y = 0,
                Radius = 1,
                Mass = 1,
                VelocityX = 1,
                VelocityY = 0
            };

            var entityB = new TestEntity
            {
                X = 1.5f,
                Y = 0,
                Radius = 1,
                Mass = 1,
                VelocityX = -1,
                VelocityY = 0
            };

            var collisionHandler = new BaseCollision();

            collisionHandler.ResolveCollision(entityA, entityB);

            Assert.AreNotEqual(entityA.X, 0); 
            Assert.AreNotEqual(entityB.X, 1.5f); 
            Assert.AreNotEqual(entityA.VelocityX, 1); 
            Assert.AreNotEqual(entityB.VelocityX, -1); 


            Assert.AreEqual(entityA.Y, 0);
            Assert.AreEqual(entityB.Y, 0);
        }
    }
}
