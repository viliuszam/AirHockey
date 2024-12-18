using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls
{
    public class StandardWall : Wall
    {
        private const float BounceFactor = 0.2f;
        private bool Movable;

        // Updated constructor: Accept FlyweightWall and pass it to the base Wall constructor
        public StandardWall(int id, FlyweightWall flyweight, bool movable)
            : base(id, flyweight)  // Pass the flyweight to the Wall constructor
        {
            Movable = movable;
        }

        public override void Update()
        {
            // Custom update logic for StandardWall (if any)
        }
    }
}
