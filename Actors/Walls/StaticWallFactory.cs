using AirHockey.Actors.Walls.WallBuilder;
namespace AirHockey.Actors.Walls
{
    public class StaticWallFactory : AbstractWallFactory
    {
        public override Wall CreateWall(int id, float width, float height, string type, float x, float y)
        {
            StaticWallBuilder buider = new StaticWallBuilder();
            return buider
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