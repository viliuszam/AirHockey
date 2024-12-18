using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls
{
    public class QuickSandWall : Wall
    {
        private const float SlowFactor = 0.5f;

        public QuickSandWall(int id, FlyweightWall flyweight)
            : base(id, flyweight)
        {
        }

        public override void Update()
        {
            // QuickSandWall has no specific update behavior for now
        }

        public float GetSlowFactor()
        {
            return SlowFactor;
        }
    }
}
