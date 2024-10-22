using AirHockey.Actors.Powerups;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace AirHockey.Actors
{
    public class Player : Entity
    {
        public string Id { get; set; }
        public string Color { get; set; }
        public float MaxSpeed = 4f;
        public float AngleFacing { get; set; }
        public string Nickname { get; set; }
        public Room Room { get; private set; }
        public Powerup? ActivePowerup { get; set; }

        public Player(string id, string color, float X, float Y, string nickname, Room room)
        {
            Id = id;
            Color = color;
            Acceleration = 0.5f;
            this.X = X;
            this.Y = Y;
            Radius = 20f;
            Mass = 1f;
            Room = room;
            AngleFacing = 0f;
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
            if (VelocityX != 0 || VelocityY != 0)
            {
                AngleFacing = (float)Math.Atan2(VelocityY, VelocityX);
            }
        }

        public void UsePowerup()
        {
            if (ActivePowerup != null)
            {
                ActivePowerup.Activate(this);
                ActivePowerup = null;
            }
        }
<<<<<<< HEAD
        /*
        public void ApplyBuff(IBuff buff)
        {
            buff.ApplyBuff(this);
        }*/
=======
>>>>>>> 3e03017d9d72a5d818c266db1c8d653278e535f0
    }

}
