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
            TeleportingWall wall1 = new TeleportingWall(1, 100, 200);
            TeleportingWall wall2 = new TeleportingWall(2, 100, 200);
            Puck puck = new Puck();

            _teleportCollision.ResolveCollision(wall1, puck);

            Assert.AreEqual(427.5f, puck.X);
            Assert.AreEqual(270.5f, puck.Y);
        }

        [Test()]
        public void ResolveCollision_TeleportingWallLinked_TeleportEntity()
        {
            TeleportingWall wall1 = new TeleportingWall(1, 100, 200);
            TeleportingWall wall2 = new TeleportingWall(2, 100, 200);
            wall1.LinkWall(wall2);
            Puck puck = new Puck();

            _teleportCollision.ResolveCollision(wall1, puck);

            Assert.AreEqual(wall2.X + wall2.Width / 2, puck.X);
            Assert.AreEqual(wall2.Y + wall2.Height / 2, puck.Y);
            Assert.AreEqual(puck, wall1.GetLast());
            Assert.AreEqual(puck, wall2.GetLast());
        }

        [Test()]
        public void ResolveCollision_TeleportingWallAlreadyTeleported_NoTeleport()
        {
            TeleportingWall wall1 = new TeleportingWall(1, 100, 200);
            TeleportingWall wall2 = new TeleportingWall(2, 100, 200);
            wall1.LinkWall(wall2);
            Puck puck = new Puck();

            _teleportCollision.ResolveCollision(wall1, puck);
            _teleportCollision.ResolveCollision(wall1, puck);

            Assert.AreEqual(wall2.X + wall2.Width / 2, puck.X);
            Assert.AreEqual(wall2.Y + wall2.Height / 2, puck.Y);
        }

        [Test()]
        public void ResolveCollision_WithWall_NoTeleport()
        {
            TeleportingWall teleportingWall = new TeleportingWall(1, 100, 200);
            StandardWall staticWall = new StandardWall(2, 100, 200, moveable: false);

            _teleportCollision.ResolveCollision(teleportingWall, staticWall);

            Assert.IsNull(teleportingWall.GetLast(), "Last teleported entity should remain null.");
        }
        [Test()]
        public void ResolveCollision_TeleportingWallsLinked_NoTeleportWhenCollidingWithEachOther()
        {
            TeleportingWall wall1 = new TeleportingWall(1, 100, 200);
            TeleportingWall wall2 = new TeleportingWall(2, 100, 200);

            wall1.LinkWall(wall2);

            _teleportCollision.ResolveCollision(wall1, wall2);

            Assert.IsNull(wall1.GetLast(), "Last teleported entity for wall1 should be null.");
            Assert.IsNull(wall2.GetLast(), "Last teleported entity for wall2 should be null.");
        }
        [Test()]
        public void ResolveCollision_TeleportingWallsGetLastIsNotOther()
        {
            TeleportingWall wall1 = new TeleportingWall(1, 100, 200);
            TeleportingWall wall2 = new TeleportingWall(2, 12, 241);
            TeleportingWall wall3 = new TeleportingWall(3, 100, 266);
            wall1.LinkWall(wall3);
            wall1.SetLast(wall2);

            _teleportCollision.ResolveCollision(wall1, wall2);

            Assert.IsNull(wall2.GetLast(), "Last teleported entity for wall1 should be null.");
            Assert.IsNull(wall2.GetLast(), "Last teleported entity for wall2 should be null.");
        }
        [Test()]
        public void ResolveCollision_TeleportingWallsGetLastIsOther2()
        {
            TeleportingWall wall1 = new TeleportingWall(1, 100, 200);
            TeleportingWall wall2 = new TeleportingWall(2, 100, 200);

            wall1.SetLast(null);

            _teleportCollision.ResolveCollision(wall1, wall2);

            Assert.IsNull(null, "Last teleported entity for wall1 should be null.");
            Assert.IsNull(wall2.GetLast(), "Last teleported entity for wall2 should be null.");
        }


    }
}
