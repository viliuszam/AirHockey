namespace AirHockey.Actors.Walls
{
    public class StaticWallFactory : AbstractWallFactory
    {
        public override Wall CreateWall(int id, float width, float height, string type)
        {
            switch (type)
            {
                case "Teleporting":
                    Wall Twall = new TeleportingWall(id, width, height);
                    Twall.Mass = 0f;
                    Twall.Acceleration = 0f;
                    return Twall;
                case "QuickSand":
                    Wall QWall = new QuickSandWall(id, width, height);
                    QWall.Mass = 0f;
                    QWall.Acceleration = 0f;
                    return QWall;
                case "Standard":
                    Wall Swall = new StandardWall(id, width, height, false);
                    Swall.Mass = 500f;
                    Swall.Acceleration = 0f;
                    return Swall;
                case "Bouncy":
                    Wall Bwall = new BouncyWall(id, width, height, false);
                    Bwall.Mass = 500f;
                    Bwall.Acceleration = 0f;
                    return Bwall;
                default:
                    throw new ArgumentException("Invalid wall type");
            }
        }

        
    }
}