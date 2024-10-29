using AirHockey.Actors.Walls;
using AirHockey.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Walls.Tests
{
    [TestFixture]
    public class TeleportingWallTests
    {
        [Test]
        public void TeleportingWall_LinkWithOtherWall_UpdatesLinkedWall()
        {
            var wall1 = new TeleportingWall(1, 100, 50);
            var wall2 = new TeleportingWall(2, 100, 50);

            bool linked = wall1.LinkWall(wall2);

            Assert.IsTrue(linked);
            Assert.AreEqual(wall2, wall1.LinkedWall);
            Assert.AreEqual(wall1, wall2.LinkedWall);
            Assert.IsTrue(wall1.IsLinked);
            Assert.IsFalse(wall2.IsLinked);
        }

        [Test]
        public void TeleportingWall_LinkWithSelf_ReturnsFalse()
        {
            var wall = new TeleportingWall(1, 100, 50);

            bool linked = wall.LinkWall(wall);

            Assert.IsFalse(linked);
            Assert.IsNull(wall.LinkedWall);
            Assert.IsFalse(wall.IsLinked);
        }

        [Test]
        public void TeleportingWall_LinkAlreadyLinkedWall_ReturnsFalse()
        {
            var wall1 = new TeleportingWall(1, 100, 50);
            var wall2 = new TeleportingWall(2, 100, 50);
            var wall3 = new TeleportingWall(3, 100, 50);

            wall1.LinkWall(wall2);  // wall1 and wall2 are now linked

            bool linked = wall1.LinkWall(wall3);

            Assert.IsFalse(linked, "Expected linking to fail when wall is already linked.");
            Assert.AreEqual(wall2, wall1.LinkedWall, "Linked wall should remain the same.");
        }

        [Test]
        public void TeleportingWall_Update_ClearsLastTeleportedEntity()
        {
            var wall = new TeleportingWall(1, 100, 50);
            var puck = new Puck();

            wall.SetLast(puck);
            Assert.AreEqual(puck, wall.GetLast(), "Expected the last teleported entity to be set.");

            wall.Update();

            Assert.IsNull(wall.GetLast(), "Expected last teleported entity to be cleared after Update.");
        }

        [Test]
        public void TeleportingWall_TrackLastTeleportedEntity_UpdatesLastEntity()
        {
            var wall = new TeleportingWall(1, 100, 50);
            var puck = new Puck();

            wall.SetLast(puck);

            Assert.AreEqual(puck, wall.GetLast());
        }
    }
}
