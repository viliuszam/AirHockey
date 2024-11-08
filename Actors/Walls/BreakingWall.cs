namespace AirHockey.Actors.Walls
{
    public class BreakingWall : Wall
    {
        public float reductionAmount = 0.1f;
        private bool alive = true;

        public BreakingWall(int id, float width, float height)
            : base(id, width, height)
        {
        }

        public override void Update()
        {
            if(alive){
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
            Width = 0;
            Height = 0;
            X = -1000;
            Y = -1000;
        }
    }
}