using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls
{
    public class BreakingWall : Wall
    {
        public float reductionAmount = 0.1f;
        private bool alive = true;

        // Updated constructor: Accept FlyweightWall and pass it to the base Wall constructor
        public BreakingWall(int id, FlyweightWall flyweight)
            : base(id, flyweight)  // Pass the flyweight to the Wall constructor
        {
        }

        public override void Update()
        {
            if (alive)
            {
                ApplyFriction();
                Move();
                if (Width <= 10 || Height <= 10)
                {
                    alive = false;
                    Disappear();
                }
            }
        }

        public void Disappear()
        {
            X = -1000;
            Y = -1000;
        }
    }
}
