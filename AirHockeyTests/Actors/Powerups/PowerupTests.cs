using AirHockey.Actors.Powerups;
using AirHockey.Actors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Powerups.Tests
{
    public class PowerupTests
    {
        [Test]
        public void Powerup_Collision_ActivatesForPlayer()
        {
            var powerup = new DashPowerup(10, 10, 1);
            var player = new Player("TestPlayer", "red", 10, 10, "Tester", new Room("TestRoom"));

            powerup.ResolveCollision(player);

            Assert.IsTrue(powerup.IsActive == false);
            Assert.AreEqual(powerup, player.ActivePowerup);
        }

        [Test]
        public void Powerup_Collision_DoesNotActivateIfPlayerHasPowerup()
        {
            var powerup = new DashPowerup(10, 10, 1);
            var player = new Player("TestPlayer", "red", 10, 10, "Tester", new Room("TestRoom"));
            player.ActivePowerup = new DashPowerup(5, 5, 2);

            powerup.ResolveCollision(player);

            Assert.IsTrue(powerup.IsActive);
            Assert.AreNotEqual(powerup, player.ActivePowerup);
        }

        [Test]
        public void Powerup_IsColliding_ReturnsCorrectCollisionState()
        {
            var powerup = new DashPowerup(100, 100, 1);
            var exclusionZone = new Rectangle(50, 50, 100, 100);

            bool isColliding = powerup.IsColliding(exclusionZone);

            Assert.IsTrue(isColliding);
        }

        [Test]
        public void Powerup_IsColliding_ReturnsFalseForNonOverlappingZone()
        {
            var powerup = new DashPowerup(100, 100, 1);
            var exclusionZone = new Rectangle(200, 200, 100, 100);

            bool isColliding = powerup.IsColliding(exclusionZone);

            Assert.IsFalse(isColliding);
        }
    }
}
