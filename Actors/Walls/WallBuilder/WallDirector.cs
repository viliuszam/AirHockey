namespace AirHockey.Actors.Walls.WallBuilder
{
    public class WallDirector
    {
        private IWallBuilder _builder;

        public WallDirector(IWallBuilder builder)
        {
            _builder = builder;
        }

        public Wall BuildWall(int id, float width, float height, string type, float x, float y)
        {
            return _builder
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