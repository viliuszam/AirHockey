namespace AirHockey.Actors.Walls
{
    public class DynamicWallFactory : AbstractWallFactory
    {
        public override Wall CreateWall(int id, float width, float height, string type)
        {
            switch (type)
            {
                case "Bouncy":
                    Wall Bwall = new BouncyWall(id, width, height, true);
                    Bwall.Mass = 0.1f;
                    Bwall.VelocityX = 0f;
                    Bwall.VelocityY = 0f;
                    Bwall.Acceleration = 0.75f;
                    return Bwall;
                case "Scrolling":
                    Wall Swall = new ScrollingWall(id, width, height);
                    Swall.Mass = 1.0f;
                    Swall.VelocityX = 0f;
                    Swall.VelocityY = 0f;
                    Swall.Acceleration = 0.75f;
                    return Swall;
                default:
                    throw new ArgumentException("Invalid wall type");
            }
        }
    }
}