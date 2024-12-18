using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls
{
    public class BouncyWall : Wall
    {
        private const float BounceFactor = 0.99f;
        private bool Movable;

        // Updated constructor: Accept FlyweightWall and pass it to the base Wall constructor
        public BouncyWall(int id, FlyweightWall flyweight, bool movable)
            : base(id, flyweight)  // Pass the flyweight to the Wall constructor
        {
            Movable = movable;
        }

        public override void Update()
        {
            if (Movable)
            {
                ApplyFriction();
                Move();
            }
        }

        public bool getMove()
        {
            return Movable;
        }

        public float GetBounce()
        {
            return BounceFactor;
        }
    }
}
