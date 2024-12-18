using System;
using System.Diagnostics;
using AirHockey.Actors.Walls.Flyweight;
using NUnit.Framework;

namespace AirHockey.Actors.Walls.WallBuilder.Tests
{
    [TestFixture]
    public class DynamicWallBuilderTests
    {
        [Test]
        public void DynamicWallBuilder_PerformanceTest()
        {
            // Initialize FlyweightFactory
            var flyweightFactory = new FlyweightFactory();

            // Pass the FlyweightFactory to the builder
            var builder = new DynamicWallBuilder(flyweightFactory);
            const int wallCount = 1000;

            // Measure time
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Measure memory before
            long memoryBefore = GC.GetTotalMemory(true);

            // Process walls in batches to manage memory better
            int batchSize = 100;
            for (int i = 0; i < wallCount; i++)
            {
                var wall = builder.SetId(i)
                                  .SetType("Bouncy")
                                  .SetDimensions(100, 50)
                                  .SetPosition(i * 10, i * 20)
                                  .SetVelocity(5, 10)
                                  .SetAcceleration()
                                  .SetMass()
                                  .Build();

            }

            // Measure memory after
            long memoryAfter = GC.GetTotalMemory(true);

            stopwatch.Stop();

            // Output performance metrics
            Console.WriteLine($"DynamicWallBuilder - Time Taken: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"DynamicWallBuilder - Memory Used: {memoryAfter - memoryBefore} bytes");

            // Assert performance (example thresholds)
            Assert.LessOrEqual(stopwatch.ElapsedMilliseconds, 1000, "Time exceeds acceptable threshold.");
            Assert.LessOrEqual(memoryAfter - memoryBefore, 10_000_000, "Memory usage exceeds acceptable threshold.");
        }
    }
}
