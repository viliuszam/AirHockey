using AirHockey.Actors.Walls.WallBuilder;
namespace AirHockey.Actors.Walls
{
    public class DynamicWallFactory : AbstractWallFactory
    {
        public override Wall CreateWall(int id, float width, float height, string type, float x, float y)
        {
            WallDirector director = new WallDirector(new DynamicWallBuilder());
            return director.BuildWall(id, width, height, type, x, y);
        }
    }
}