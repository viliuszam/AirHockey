using AirHockey.Actors.Walls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockeyTests.Actors.Walls
{
    [TestFixture]
    public class AbstractWallFactoryTests
    {
        [Test]
        public void GetFactory_ReturnsDynamicWallFactory_WhenIsDynamicIsTrue()
        {
            var factory = AbstractWallFactory.GetFactory(true);

            Assert.IsInstanceOf<DynamicWallFactory>(factory, "Expected DynamicWallFactory when isDynamic is true.");
        }

        [Test]
        public void GetFactory_ReturnsStaticWallFactory_WhenIsDynamicIsFalse()
        {
            var factory = AbstractWallFactory.GetFactory(false);

            Assert.IsInstanceOf<StaticWallFactory>(factory, "Expected StaticWallFactory when isDynamic is false.");
        }
    }
}
