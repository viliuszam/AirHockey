using AirHockey.Actors.Powerups;
namespace AirHockey.Actors
{
    public class Player : Entity
    {
        public string Id { get; set; }
        public string Color { get; set; }
        public float MaxSpeed = 4f;

        public string Nickname { get; set; }

        public Powerup ActivePowerup { get; set; }

        public Player(string id, string color, float X, float Y, string nickname)
        {
            Id = id;
            Color = color;
            Acceleration = 0.5f;
            this.X = X;
            this.Y = Y;
            Radius = 20f;
            Mass = 1f;
            Nickname = nickname;
        }

        public override void Update()
        {
            Move();
            ApplyFriction();
            VelocityX = Math.Clamp(VelocityX, -MaxSpeed, MaxSpeed);
            VelocityY = Math.Clamp(VelocityY, -MaxSpeed, MaxSpeed);
        }

        public void Accelerate(float xDirection, float yDirection)
        {
            VelocityX += xDirection * Acceleration;
            VelocityY += yDirection * Acceleration;
        }

        public void UsePowerup()
        {
            if (ActivePowerup != null)
            {
                ActivePowerup.Activate(this);
                ActivePowerup = null;
            }
        }
    }

}
