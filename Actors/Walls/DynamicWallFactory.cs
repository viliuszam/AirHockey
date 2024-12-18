using AirHockey.Actors.Walls.WallBuilder;
using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls
{
    public class DynamicWallFactory : AbstractWallFactory
    {
        private readonly FlyweightFactory _flyweightFactory;

        public DynamicWallFactory(FlyweightFactory flyweightFactory)
        {
            _flyweightFactory = flyweightFactory;
        }

        public override Wall CreateWall(int id, float width, float height, string type, float x, float y)
        {
            DynamicWallBuilder builder = new DynamicWallBuilder(_flyweightFactory);
            return builder
                .SetId(id)
                .SetDimensions(width, height)
                .SetType(type)
                .SetPosition(x, y)
                .SetVelocity()
                .SetAcceleration()
                .SetMass()
                .Build();
        }
    }
}
