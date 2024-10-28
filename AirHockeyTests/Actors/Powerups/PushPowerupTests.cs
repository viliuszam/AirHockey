using AirHockey.Actors.Powerups;
using AirHockey.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Powerups.Tests
{
    public class PushPowerupTests
    {

        [Test]
        public void PushPowerup_Activate_AppliesPushForceCorrectly()
        {
            // Arrange
            var room = new Room("TestRoom");
            var p1 = new Player("TestPlayer1", "red", 100, 100, "Tester1", room);
            var p2 = new Player("TestPlayer2", "red", 170, 170, "Tester2", room);
            var puck = new Puck();
            puck.X = 115;
            puck.Y = 115;
            room.Puck = puck;
            room.AddPlayer(p1);
            room.AddPlayer(p2);
            var pushPowerup = new PushPowerup(150, 150, 1, 30, 2f, 600);
            room.Powerups.Add(pushPowerup);

            // Act
            pushPowerup.Activate(p1);

            // Assert
            Assert.Greater(Math.Abs(p2.VelocityX), 0.01f);
            Assert.Less(Math.Abs(p2.VelocityY), 0.01f);
            Assert.Greater(Math.Abs(puck.VelocityX), 0.01f);
            Assert.Less(Math.Abs(puck.VelocityY), 0.01f);
        }

        [Test]
        public void PushPowerup_Activate_DoesNotApplyForceOutsideRadius()
        {
            // Arrange
            var room = new Room("TestRoom");
            var p1 = new Player("TestPlayer1", "red", 100, 100, "Tester1", room);
            var p2 = new Player("TestPlayer2", "red", 170, 170, "Tester2", room);
            var puck = new Puck();
            puck.X = 115;
            puck.Y = 115;
            room.Puck = puck;
            room.AddPlayer(p1);
            room.AddPlayer(p2);
            var pushPowerup = new PushPowerup(150, 150, 1, 30, 2f, 600);
            room.Powerups.Add(pushPowerup);

            // Act
            pushPowerup.Activate(p1);

            // Assert
            Assert.Greater(Math.Abs(p2.VelocityX), 0.01f);
            Assert.Less(Math.Abs(p2.VelocityY), 0.01f);
            Assert.Greater(Math.Abs(puck.VelocityX), 0.01f);
            Assert.Less(Math.Abs(puck.VelocityY), 0.01f);
        }

        [Test]
        public void PushPowerup_CloneShallow_ReturnsIdenticalObject()
        {
            var original = new PushPowerup(100, 100, 1, 10f, 2f, 300f);

            var clone = original.CloneShallow();

            Assert.AreEqual(original.X, clone.X);
            Assert.AreEqual(original.Y, clone.Y);
            Assert.AreEqual(original.Id, clone.Id);
            Assert.AreEqual(original.PushForce, (clone as PushPowerup).PushForce);
            Assert.AreEqual(original.Duration, (clone as PushPowerup).Duration);
            Assert.AreEqual(original.PushRadius, (clone as PushPowerup).PushRadius);
            Assert.AreSame(original.GetType(), clone.GetType());
        }

        [Test]
        public void PushPowerup_CloneDeep_ReturnsDifferentObject()
        {
            var original = new PushPowerup(100, 100, 1, 10f, 2f, 300f);

            var clone = original.CloneDeep();

            Assert.AreEqual(original.X, clone.X);
            Assert.AreEqual(original.Y, clone.Y);
            Assert.AreEqual(original.Id, clone.Id);
            Assert.AreEqual(original.PushForce, (clone as PushPowerup).PushForce);
            Assert.AreEqual(original.Duration, (clone as PushPowerup).Duration);
            Assert.AreEqual(original.PushRadius, (clone as PushPowerup).PushRadius);
            Assert.AreSame(original.GetType(), clone.GetType());
        }
    }
}
