namespace AirHockey.Actors.Walls
{
    public class StandardWall : Wall
    {
        private const float BounceFactor = 0.2f;
        private bool Movable;
        public StandardWall(int id, float width, float height, bool moveable)
            : base(id, width, height)
        {
            Movable = moveable;
        }
        

        public override void Update(){}

    }
}