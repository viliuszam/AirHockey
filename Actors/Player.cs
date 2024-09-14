namespace AirHockey.Actors
{
    public class Player : Entity
    {
        public string Id { get; set; }
        public string Color { get; set; }
        private readonly float maxSpeed = 4f;

        public Player(string id, string color, float X, float Y)
        {
            Id = id;
            Color = color;
            Acceleration = 0.5f;
            this.X = X;
            this.Y = Y;
            Radius = 20f;
            Mass = 1f;
        }

        public override void Update()
        {
            Move();
            ApplyFriction();
            VelocityX = Math.Clamp(VelocityX, -maxSpeed, maxSpeed);
            VelocityY = Math.Clamp(VelocityY, -maxSpeed, maxSpeed);
        }

        public void Accelerate(float xDirection, float yDirection)
        {
            VelocityX += xDirection * Acceleration;
            VelocityY += yDirection * Acceleration;
        }
    }

}
