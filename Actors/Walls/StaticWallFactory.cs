using AirHockey.Actors.Walls.WallBuilder;
using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls
{
    public class StaticWallFactory : AbstractWallFactory
    {
        private readonly FlyweightFactory _flyweightFactory;

        public StaticWallFactory(FlyweightFactory flyweightFactory)
        {
            _flyweightFactory = flyweightFactory;
        }

        public override Wall CreateWall(int id, float width, float height, string type, float x, float y)
        {
            StaticWallBuilder builder = new StaticWallBuilder(_flyweightFactory);
            return builder
                .SetId(id)
                .SetDimensions(width, height)
                .SetType(type)
                .SetPosition(x, y)
                .SetAcceleration()
                .SetMass()
                .Build();
        }
    }
}
