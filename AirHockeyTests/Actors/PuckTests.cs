using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Tests
{
    [TestFixture]
    public class PuckTests
    {
        private Puck puck;

        [SetUp]
        public void Setup()
        {
            puck = new Puck();
        }

        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(puck.Friction, Is.EqualTo(0.98f));
                Assert.That(puck.X, Is.EqualTo(427.5f));
                Assert.That(puck.Y, Is.EqualTo(270.5f));
                Assert.That(puck.Radius, Is.EqualTo(15f));
                Assert.That(puck.Mass, Is.EqualTo(0.5f));
                Assert.That(puck.MaxSpeed, Is.EqualTo(15f));
                Assert.That(puck.VelocityX, Is.EqualTo(0f));
                Assert.That(puck.VelocityY, Is.EqualTo(0f));
            });
        }

        [Test]
        public void Update_AppliesFriction()
        {
            // Set initial velocities
            puck.VelocityX = 10f;
            puck.VelocityY = 5f;

            float expectedVelocityX = 10f * puck.Friction;
            float expectedVelocityY = 5f * puck.Friction;

            puck.Update();

            Assert.Multiple(() =>
            {
                Assert.That(puck.VelocityX, Is.EqualTo(expectedVelocityX));
                Assert.That(puck.VelocityY, Is.EqualTo(expectedVelocityY));
            });
        }

        [Test]
        public void Update_MovesPosition()
        {
            puck.VelocityX = 10f;
            puck.VelocityY = 5f;
            float initialX = puck.X;
            float initialY = puck.Y;

            puck.Update();

            Assert.Multiple(() =>
            {
                Assert.That(puck.X, Is.EqualTo(initialX + (10f * puck.Friction)));
                Assert.That(puck.Y, Is.EqualTo(initialY + (5f * puck.Friction)));
            });
        }

        [Test]
        public void Update_CapsVelocityAtMaxSpeed()
        {
            // Set velocity higher than max speed
            puck.VelocityX = 20f;
            puck.VelocityY = 20f;

            puck.Update();

            float currentSpeed = (float)Math.Sqrt(
                puck.VelocityX * puck.VelocityX +
                puck.VelocityY * puck.VelocityY
            );

            Assert.That(currentSpeed, Is.LessThanOrEqualTo(puck.MaxSpeed));
        }

        [Test]
        public void Update_PreservesVelocityDirectionWhenCapping()
        {
            puck.VelocityX = 20f;
            puck.VelocityY = 10f;
            float initialAngle = (float)Math.Atan2(puck.VelocityY, puck.VelocityX);

            puck.Update();

            float finalAngle = (float)Math.Atan2(puck.VelocityY, puck.VelocityX);
            Assert.That(finalAngle, Is.EqualTo(initialAngle).Within(0.0001f));
        }

        [Test]
        public void Update_DoesNotCapVelocityBelowMaxSpeed()
        {
            float speedBelowMax = puck.MaxSpeed / 2;
            puck.VelocityX = speedBelowMax;
            puck.VelocityY = 0f;

            float initialVelocityX = puck.VelocityX;
            puck.Update();

            // Should only be affected by friction, not capping
            Assert.That(puck.VelocityX, Is.EqualTo(initialVelocityX * puck.Friction));
        }

        [Test]
        public void Puck_InheritsFromEntity()
        {
            Assert.That(puck, Is.InstanceOf<Entity>());
        }

        [TestCase(0f, 0f)]
        [TestCase(5f, 0f)]
        [TestCase(0f, 5f)]
        [TestCase(-5f, 0f)]
        [TestCase(0f, -5f)]
        [TestCase(3f, 4f)]
        [TestCase(-3f, -4f)]
        public void Update_HandlesVariousVelocities(float velocityX, float velocityY)
        {
            puck.VelocityX = velocityX;
            puck.VelocityY = velocityY;

            float initialSpeed = (float)Math.Sqrt(velocityX * velocityX + velocityY * velocityY);
            puck.Update();
            float finalSpeed = (float)Math.Sqrt(
                puck.VelocityX * puck.VelocityX +
                puck.VelocityY * puck.VelocityY
            );

            if (initialSpeed > puck.MaxSpeed)
            {
                Assert.That(finalSpeed, Is.LessThanOrEqualTo(puck.MaxSpeed));
            }
            else
            {
                Assert.That(finalSpeed, Is.EqualTo(initialSpeed * puck.Friction).Within(0.0001f));
            }
        }

        [Test]
        public void Puck_CollisionBehaviorInheritedFromEntity()
        {
            var otherPuck = new Puck
            {
                X = puck.X + puck.Radius + 5f, // Slightly more than radius apart
                Y = puck.Y
            };

            bool isColliding = puck.IsColliding(otherPuck);
            Assert.That(isColliding, Is.True);
        }

        [Test]
        public void Puck_StandStill()
        {
            puck.VelocityX = 0f;
            puck.VelocityY = 0f;
            float initialX = puck.X;
            float initialY = puck.Y;

            for(int i = 0; i < 10000; i++)
                puck.Update();

            Assert.Multiple(() =>
            {
                Assert.That(puck.X, Is.EqualTo(initialX));
                Assert.That(puck.Y, Is.EqualTo(initialY));
                Assert.That(puck.VelocityX, Is.EqualTo(0f));
                Assert.That(puck.VelocityY, Is.EqualTo(0f));
            });
        }
    }
}
