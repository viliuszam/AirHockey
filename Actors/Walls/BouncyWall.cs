namespace AirHockey.Actors.Walls
{
    public class BouncyWall : Wall
    {
        private const float BounceFactor = 0.99f;
        private bool Movable;

        public BouncyWall(int id, float width, float height, bool moveable)
            : base(id, width, height)
        {
            Movable = moveable;

        }

        public override void Update(){
            if(Movable){
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