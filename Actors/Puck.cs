namespace AirHockey.Actors
{
    public class Puck : Entity
    {
        private readonly float maxSpeed = 15f;

        public Puck()
        {
            Friction = 0.98f;
            X = 427.5f;
            Y = 270.5f;
            Radius = 15f;
            Mass = 0.5f;
        }

        public override void Update()
        {
            ApplyFriction();
            Move();
            CapVelocity();
        }

        private void CapVelocity()
        {
            float currentSpeed = (float)Math.Sqrt(VelocityX * VelocityX + VelocityY * VelocityY);
            if (currentSpeed > maxSpeed)
            {
                float scale = maxSpeed / currentSpeed;
                VelocityX *= scale;
                VelocityY *= scale;
            }
        }
    }
}
