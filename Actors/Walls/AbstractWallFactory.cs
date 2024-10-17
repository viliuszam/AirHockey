namespace AirHockey.Actors.Walls
{
    public abstract class AbstractWallFactory
    {
        public abstract Wall CreateWall(int id, float width, float height, string type, float x, float y);

        public static AbstractWallFactory GetFactory(bool isDynamic)
        {
            if (isDynamic)
            {
                return new DynamicWallFactory();
            }
            else
            {
                return new StaticWallFactory();
            }
        }
    }
}