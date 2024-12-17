using System.Drawing;

namespace AirHockey.Actors.Powerups
{
    public abstract class Powerup : Entity, ICloneable
    {
        private System.Timers.Timer _timer;
        public int Id { get; set; }
        public float Duration { get; protected init;}
        public bool IsActive { get; private set; }

        public Powerup(float x, float y, int id)
        {
            X = x;
            Y = y;
            Radius = 20f;
            IsActive = true;
            Id = id;
        }

        public void ResolveCollision(Player other)
        {
            if (IsActive && other.ActivePowerup == null)
            {
                IsActive = false;
                other.ActivePowerup = this;
                Console.WriteLine($"Player {other.Nickname} now has a powerup!");
            }
        }

        public bool IsColliding(Rectangle exclusionZone)
        {
            float closestX = Math.Max(exclusionZone.X, Math.Min(this.X, exclusionZone.X + exclusionZone.Width));
            float closestY = Math.Max(exclusionZone.Y, Math.Min(this.Y, exclusionZone.Y + exclusionZone.Height));

            float distanceX = this.X - closestX;
            float distanceY = this.Y - closestY;

            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared < (this.Radius * this.Radius);
        }

        public void Activate(Player player)
        {
            ApplyEffect(player);
            Console.WriteLine($"Powerup {this.GetType()} has a duration of {this.Duration}");
            
            _timer = new System.Timers.Timer(Duration * 1000);
            _timer.Elapsed += (sender, e) =>
            {
                RemoveEffect(player);
                _timer.Stop();
            };
            _timer.Start();
        }

        protected virtual void ApplyEffect(Player player)
        {
            
        }
        
        protected virtual void RemoveEffect(Player player)
        {
            
        }

        public abstract Powerup CloneShallow();

        public abstract Powerup CloneDeep();

        public virtual Type GetBaseType()
        {
            return this.GetType();
        }

        public object Clone()
        {
            return (Powerup)this.MemberwiseClone();
        }
    }
}
