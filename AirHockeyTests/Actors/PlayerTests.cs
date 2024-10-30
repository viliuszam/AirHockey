using AirHockey.Actors.Powerups;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Tests
{
    [TestFixture]
    public class PlayerTests
    {
        private Player player;
        private Room room;
        private const string TEST_ID = "test-id";
        private const string TEST_COLOR = "#FF0000";
        private const float TEST_X = 100f;
        private const float TEST_Y = 100f;
        private const string TEST_NICKNAME = "TestPlayer";

        [SetUp]
        public void Setup()
        {
            room = new Room("TEST123");
            player = new Player(TEST_ID, TEST_COLOR, TEST_X, TEST_Y, TEST_NICKNAME, room);
        }

        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(player.Id, Is.EqualTo(TEST_ID));
                Assert.That(player.Color, Is.EqualTo(TEST_COLOR));
                Assert.That(player.X, Is.EqualTo(TEST_X));
                Assert.That(player.Y, Is.EqualTo(TEST_Y));
                Assert.That(player.Nickname, Is.EqualTo(TEST_NICKNAME));
                Assert.That(player.Room, Is.EqualTo(room));
                Assert.That(player.Acceleration, Is.EqualTo(0.5f));
                Assert.That(player.Radius, Is.EqualTo(20f));
                Assert.That(player.Mass, Is.EqualTo(1f));
                Assert.That(player.AngleFacing, Is.EqualTo(0f));
                Assert.That(player.MaxSpeed, Is.EqualTo(4f));
                Assert.That(player.ActivePowerup, Is.Null);
            });
        }

        [Test]
        public void Update_ClampsVelocityToMaxSpeed()
        {
            player.VelocityX = 10f;
            player.VelocityY = -10f;

            player.Update();

            Assert.Multiple(() =>
            {
                Assert.That(player.VelocityX, Is.EqualTo(player.MaxSpeed));
                Assert.That(player.VelocityY, Is.EqualTo(-player.MaxSpeed));
            });
        }

        [Test]
        public void Update_AppliesFrictionAndMovement()
        {
            player.VelocityX = 2f;
            player.VelocityY = 2f;
            float initialX = player.X;
            float initialY = player.Y;
            float expectedVelocityAfterFriction = 2f * player.Friction;

            player.Update();

            Assert.Multiple(() =>
            {
                Assert.That(player.X, Is.Not.EqualTo(initialX));
                Assert.That(player.Y, Is.Not.EqualTo(initialY));
                Assert.That(player.VelocityX, Is.EqualTo(expectedVelocityAfterFriction));
                Assert.That(player.VelocityY, Is.EqualTo(expectedVelocityAfterFriction));
            });
        }

        [Test]
        public void Accelerate_UpdatesVelocityAndAngle()
        {
            float xDirection = 1f;
            float yDirection = 1f;
            float initialVelocityX = player.VelocityX;
            float initialVelocityY = player.VelocityY;

            player.Accelerate(xDirection, yDirection);

            Assert.Multiple(() =>
            {
                Assert.That(player.VelocityX, Is.EqualTo(initialVelocityX + xDirection * player.Acceleration));
                Assert.That(player.VelocityY, Is.EqualTo(initialVelocityY + yDirection * player.Acceleration));
                Assert.That(player.AngleFacing, Is.EqualTo((float)Math.Atan2(player.VelocityY, player.VelocityX)));
            });
        }

        [Test]
        public void Accelerate_ZeroVelocity_DoesNotUpdateAngle()
        {
            float initialAngle = player.AngleFacing;
            player.Accelerate(0f, 0f);

            Assert.That(player.AngleFacing, Is.EqualTo(initialAngle));
        }

        [Test]
        public void UsePowerup_WithNoPowerup_DoesNothing()
        {
            player.ActivePowerup = null;
            Assert.DoesNotThrow(() => player.UsePowerup());
            Assert.That(player.ActivePowerup, Is.Null);
        }

        [Test]
        public void UsePowerup_WithActivePowerup_ActivatesAndClearsPowerup()
        {
            var powerupMock = new Mock<Powerup>(30f, 30f, 1);
            player.ActivePowerup = powerupMock.Object;

            player.UsePowerup();

            powerupMock.Verify(p => p.Activate(player), Times.Once);
            Assert.That(player.ActivePowerup, Is.Null);
        }

        [TestCase(1f, 0f)]
        [TestCase(0f, 1f)]
        [TestCase(-1f, 0f)]
        [TestCase(0f, -1f)]
        [TestCase(1f, 1f)]
        public void Accelerate_DifferentDirections_UpdatesAngleCorrectly(float xDir, float yDir)
        {
            player.VelocityX = 0;
            player.VelocityY = 0;
            player.Accelerate(xDir, yDir);

            float expectedAngle = (float)Math.Atan2(
                yDir * player.Acceleration,
                xDir * player.Acceleration
            );

            Assert.That(player.AngleFacing, Is.EqualTo(expectedAngle));
        }

        [Test]
        public void Player_InheritsFromEntity()
        {
            Assert.That(player, Is.InstanceOf<Entity>());
        }

        [Test]
        public void Update_RespectsInheritedBehavior()
        {
            player.VelocityX = 2f;
            player.VelocityY = 2f;
            float expectedX = player.X + player.VelocityX;
            float expectedY = player.Y + player.VelocityY;

            player.Update();

            Assert.Multiple(() =>
            {
                Assert.That(Math.Abs(player.X - expectedX), Is.LessThan(0.01f));
                Assert.That(Math.Abs(player.Y - expectedY), Is.LessThan(0.01f));
            });
        }

        [Test]
        public void Player_PreservesEntityCollisionBehavior()
        {
            var otherPlayer = new Player("other-id", "blue", TEST_X + 10f, TEST_Y + 10f, "Other", room);

            bool isColliding = player.IsColliding(otherPlayer);

            Assert.That(isColliding, Is.True);
        }

        [Test]
        public void Accelerate_RespectsMaxSpeed()
        {
            for (int i = 0; i < 20; i++)
            {
                player.Accelerate(1f, 1f);
            }

            player.Update();

            Assert.Multiple(() =>
            {
                Assert.That(player.VelocityX, Is.LessThanOrEqualTo(player.MaxSpeed));
                Assert.That(player.VelocityY, Is.LessThanOrEqualTo(player.MaxSpeed));
            });
        }
    }
}
