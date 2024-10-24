using System.Drawing;

namespace AirHockey.Actors.Powerups
{
    public abstract class Powerup : Entity
    {
        public int Id { get; set; }
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

        public abstract void Activate(Player player);
    }
}
