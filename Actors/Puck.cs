namespace AirHockey.Actors
{
    public class Puck : Entity
    {
        public Puck()
        {
            Friction = 0.7f;
            // saiba stalo vidury
            this.X = 427.5f;
            this.Y = 270.5f;
        }

        public override void Update()
        {
            ApplyFriction();
            Move();
        }
    }
}
