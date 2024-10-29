using AirHockey.Actors.Powerups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey.Actors.Powerups.Tests
{
    public class PowerupFactoryTests
    {
        [Test]
        public void PowerupFactory_CreatePowerup_ReturnsCorrectType()
        {
            var factory = new PowerupFactory();

            var dashPowerup = factory.CreatePowerup(10, 10, 1, "Dash");
            var freezePowerup = factory.CreatePowerup(20, 20, 2, "Freeze");
            var pushPowerup = factory.CreatePowerup(30, 30, 3, "Push");

            Assert.IsInstanceOf<DashPowerup>(dashPowerup);
            Assert.IsInstanceOf<FreezePowerup>(freezePowerup);
            Assert.IsInstanceOf<PushPowerup>(pushPowerup);
        }

        [Test]
        public void PowerupFactory_CreatePowerup_ThrowsExceptionForInvalidType()
        {
            var factory = new PowerupFactory();

            Assert.Throws<ArgumentException>(() => factory.CreatePowerup(10, 10, 1, "InvalidType"));
        }
    }
}
