using NUnit.Framework;
using AirHockey.Actors;
using AirHockey.Actors.Walls;
using AirHockey.Strategies;

namespace AirHockey.Strategies.Tests
{
    [TestFixture()]
    public class TeleportCollisionTests
    {
        private TeleportCollision _teleportCollision;

        [SetUp]
        public void SetUp()
        {
            _teleportCollision = new TeleportCollision();
        }

        [Test()]
        public void ResolveCollision_TeleportingWallNotLinked_NoTeleport()
        {
            // Arrange: Create two teleporting walls and a puck
            TeleportingWall wall1 = new TeleportingWall(1, 100, 200);
            TeleportingWall wall2 = new TeleportingWall(2, 100, 200);
            Puck puck = new Puck(); // Use the Puck constructor for initialization

            // Act: Try to resolve collision
            _teleportCollision.ResolveCollision(wall1, puck);

            // Assert: Position of the puck should remain unchanged
            Assert.AreEqual(427.5f, puck.X); // Default initial X position
            Assert.AreEqual(270.5f, puck.Y); // Default initial Y position
        }

        [Test()]
        public void ResolveCollision_TeleportingWallLinked_TeleportEntity()
        {
            // Arrange: Create two linked teleporting walls
            TeleportingWall wall1 = new TeleportingWall(1, 100, 200);
            TeleportingWall wall2 = new TeleportingWall(2, 100, 200);
            wall1.LinkWall(wall2);
            Puck puck = new Puck(); // Use the Puck constructor for initialization

            // Act: Resolve collision to teleport
            _teleportCollision.ResolveCollision(wall1, puck);

            // Assert: Puck's position should be set to wall2's center
            Assert.AreEqual(wall2.X + wall2.Width / 2, puck.X);
            Assert.AreEqual(wall2.Y + wall2.Height / 2, puck.Y);
            Assert.AreEqual(puck, wall1.GetLast()); // Check last teleported entity
            Assert.AreEqual(puck, wall2.GetLast()); // Check last teleported entity on linked wall
        }

        [Test()]
        public void ResolveCollision_TeleportingWallAlreadyTeleported_NoTeleport()
        {
            // Arrange: Create linked teleporting walls
            TeleportingWall wall1 = new TeleportingWall(1, 100, 200);
            TeleportingWall wall2 = new TeleportingWall(2, 100, 200);
            wall1.LinkWall(wall2);
            Puck puck = new Puck(); // Use the Puck constructor for initialization

            // Act: Resolve collision to teleport the first time
            _teleportCollision.ResolveCollision(wall1, puck);
            // Now try to teleport again with the same puck
            _teleportCollision.ResolveCollision(wall1, puck);

            // Assert: Puck's position should remain unchanged
            Assert.AreEqual(wall2.X + wall2.Width / 2, puck.X);
            Assert.AreEqual(wall2.Y + wall2.Height / 2, puck.Y);
        }

        [Test()]
        public void ResolveCollision_WithWall_NoTeleport()
        {
            // Arrange: Create a teleporting wall and a static wall
            TeleportingWall teleportingWall = new TeleportingWall(1, 100, 200);
            StandardWall staticWall = new StandardWall(2, 100, 200, moveable: false);

            // Act: Resolve collision
            _teleportCollision.ResolveCollision(teleportingWall, staticWall);

            // Assert: No teleport occurs, as static wall is not an entity to teleport
            Assert.IsNull(teleportingWall.GetLast(), "Last teleported entity should remain null.");
        }
    }
}
