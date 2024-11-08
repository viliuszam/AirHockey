namespace AirHockey.Actors.Walls
{
    public abstract class AbstractWallFactory
    {
        public abstract Wall CreateWall(int id, float width, float height, string type, float x, float y);
    }
}