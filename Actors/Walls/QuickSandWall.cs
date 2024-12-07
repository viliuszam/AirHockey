namespace AirHockey.Actors.Walls
{
    public class QuickSandWall : Wall
    {
        private const float SlowFactor = 0.5f;

        public QuickSandWall(int id, float width, float height)
            : base(id, width, height)
        {
        }

        public override void Update(){}

        public float GetSlowFactor()
        {
            return SlowFactor;
        }

    }
}