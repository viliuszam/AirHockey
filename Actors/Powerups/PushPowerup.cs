namespace AirHockey.Actors.Powerups
{
    public class PushPowerup : Powerup
    {
        public float PushForce;
        public float Duration;
        public float PushRadius; 

        public PushPowerup(float x, float y, int id, float pushForce = 5f, float duration = 2f, float radius = 300f)
            : base(x, y, id)
        {
            PushForce = pushForce;
            Duration = duration;
            PushRadius = radius;
        }

        public override void Activate(Player player)
        {
            foreach (var otherPlayer in player.Room.Players)
            {
                if (otherPlayer != player && IsWithinRadius(otherPlayer))
                {
                    ApplyPushForce(otherPlayer, player.AngleFacing, PushForce);
                }
            }

            if (IsWithinRadius(player.Room.Puck))
            {
                ApplyPushForce(player.Room.Puck, player.AngleFacing, PushForce);
            }

            System.Timers.Timer timer = new(Duration * 1000);
            timer.Elapsed += (sender, e) =>
            {
                timer.Stop();
                timer.Dispose(); 
            };
            timer.Start();
        }

        private void ApplyPushForce(Entity entity, float angle, float force)
        {
            float pushX = (float)Math.Cos(angle) * force;
            float pushY = (float)Math.Sin(angle) * force;

            entity.VelocityX += pushX;
            entity.VelocityY += pushY;
        }

        private bool IsWithinRadius(Entity entity)
        {
            float distanceX = entity.X - this.X;
            float distanceY = entity.Y - this.Y;
            float distanceSquared = distanceX * distanceX + distanceY * distanceY;

            return distanceSquared <= (PushRadius * PushRadius);
        }

        public override void Update()
        {
            
        }

        public override Powerup CloneShallow()
        {
            return (PushPowerup)this.MemberwiseClone();
        }

        public override Powerup CloneDeep()
        {
            return new PushPowerup(this.X, this.Y, this.Id, PushForce, Duration, PushRadius);
        }
    }
}
