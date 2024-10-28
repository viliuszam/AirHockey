using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Tests
{
    // Concrete implementation of Entity for testing
    public class TestEntity : Entity
    {
        public override void Update()
        {
            // Simple implementation for testing
            Move();
            ApplyFriction();
        }
    }

    [TestFixture]
    public class EntityTests
    {
        private TestEntity entity;
        private TestEntity otherEntity;

        [SetUp]
        public void Setup()
        {
            entity = new TestEntity
            {
                X = 100f,
                Y = 100f,
                VelocityX = 5f,
                VelocityY = 5f,
                Radius = 10f,
                Mass = 1f,
                Friction = 0.99f,
                Acceleration = 1f
            };

            otherEntity = new TestEntity
            {
                X = 150f,
                Y = 150f,
                Radius = 10f
            };
        }

        [Test]
        public void Properties_SetInvalidValues_DefaultsToZero()
        {
            entity.X = float.PositiveInfinity;
            entity.Y = float.NegativeInfinity;
            entity.VelocityX = float.NaN;
            entity.VelocityY = float.NaN;

            Assert.Multiple(() =>
            {
                Assert.That(entity.X, Is.EqualTo(0f));
                Assert.That(entity.Y, Is.EqualTo(0f));
                Assert.That(entity.VelocityX, Is.EqualTo(0f));
                Assert.That(entity.VelocityY, Is.EqualTo(0f));
            });
        }

        [Test]
        public void ApplyFriction_ReducesVelocity()
        {
            float initialVelocityX = entity.VelocityX;
            float initialVelocityY = entity.VelocityY;

            entity.ApplyFriction();

            Assert.Multiple(() =>
            {
                Assert.That(entity.VelocityX, Is.EqualTo(initialVelocityX * entity.Friction));
                Assert.That(entity.VelocityY, Is.EqualTo(initialVelocityY * entity.Friction));
            });
        }

        [Test]
        public void Move_UpdatesPosition()
        {
            float initialX = entity.X;
            float initialY = entity.Y;

            entity.Move();

            Assert.Multiple(() =>
            {
                Assert.That(entity.X, Is.EqualTo(initialX + entity.VelocityX));
                Assert.That(entity.Y, Is.EqualTo(initialY + entity.VelocityY));
            });
        }

        [Test]
        public void IsColliding_WhenColliding_ReturnsTrue()
        {
            // Position entities close enough to collide
            entity.X = 100f;
            entity.Y = 100f;
            otherEntity.X = 110f;
            otherEntity.Y = 110f;

            bool isColliding = entity.IsColliding(otherEntity);

            Assert.That(isColliding, Is.True);
        }

        [Test]
        public void IsColliding_WhenNotColliding_ReturnsFalse()
        {
            // Position entities far apart
            entity.X = 100f;
            entity.Y = 100f;
            otherEntity.X = 200f;
            otherEntity.Y = 200f;

            bool isColliding = entity.IsColliding(otherEntity);

            Assert.That(isColliding, Is.False);
        }

        [TestCase(0f, 0f, 200f, 200f)]
        [TestCase(-100f, -100f, 100f, 100f)]
        public void ConstrainToBounds_EntityStaysWithinBounds(float minX, float minY, float maxX, float maxY)
        {
            entity.X = minX - 5f;
            entity.Y = 50f;
            entity.ConstrainToBounds(minX, minY, maxX, maxY);
            Assert.That(entity.X, Is.EqualTo(minX + entity.Radius));

            entity.X = maxX + 5f;
            entity.ConstrainToBounds(minX, minY, maxX, maxY);
            Assert.That(entity.X, Is.EqualTo(maxX - entity.Radius));

            entity.X = 50f;
            entity.Y = minY - 5f;
            entity.ConstrainToBounds(minX, minY, maxX, maxY);
            Assert.That(entity.Y, Is.EqualTo(minY + entity.Radius));

            entity.Y = maxY + 5f;
            entity.ConstrainToBounds(minX, minY, maxX, maxY);
            Assert.That(entity.Y, Is.EqualTo(maxY - entity.Radius));
        }

        [Test]
        public void ConstrainToBounds_VelocityReversesWithDamping()
        {
            float initialVelocityX = 5f;
            float initialVelocityY = 5f;
            entity.VelocityX = initialVelocityX;
            entity.VelocityY = initialVelocityY;

            // Test X velocity reversal
            entity.X = -5f; // Outside left boundary
            entity.ConstrainToBounds(0f, 0f, 200f, 200f);
            Assert.That(entity.VelocityX, Is.EqualTo(-initialVelocityX * 0.8f));

            // Test Y velocity reversal
            entity.Y = -5f; // Outside top boundary
            entity.ConstrainToBounds(0f, 0f, 200f, 200f);
            Assert.That(entity.VelocityY, Is.EqualTo(-initialVelocityY * 0.8f));
        }

        [Test]
        public void Update_ModifiesEntityState()
        {
            float initialX = entity.X;
            float initialY = entity.Y;
            float initialVelocityX = entity.VelocityX;
            float initialVelocityY = entity.VelocityY;

            entity.Update();

            Assert.Multiple(() =>
            {
                // Position should change
                Assert.That(entity.X, Is.Not.EqualTo(initialX));
                Assert.That(entity.Y, Is.Not.EqualTo(initialY));
                // Velocity should be reduced by friction
                Assert.That(entity.VelocityX, Is.EqualTo(initialVelocityX * entity.Friction));
                Assert.That(entity.VelocityY, Is.EqualTo(initialVelocityY * entity.Friction));
            });
        }
    }
}
