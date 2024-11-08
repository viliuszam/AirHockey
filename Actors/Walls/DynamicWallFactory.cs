using AirHockey.Actors.Walls.WallBuilder;
namespace AirHockey.Actors.Walls
{
    public class DynamicWallFactory : AbstractWallFactory
    {
        public override Wall CreateWall(int id, float width, float height, string type, float x, float y)
        {
            DynamicWallBuilder buider = new DynamicWallBuilder();
            return buider
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