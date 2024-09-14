namespace AirHockey.Actors
{
    public abstract class Entity
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float Acceleration { get; set; }
        public float Friction { get; set; } = 0.99f;

        public abstract void Update();

        public void ApplyFriction()
        {
            VelocityX *= Friction;
            VelocityY *= Friction;
        }

        public void Move()
        {
            X += VelocityX;
            Y += VelocityY;
        }
    }
}
