using AirHockey.Actors;
using AirHockey.Actors.Walls;

namespace AirHockey.Strategies
{
    public class WallCollision: ICollision
    {
        private const float BounceFactor = 0.2f;
        public void ResolveCollision(Entity entity, Entity other)
        {
            if (entity is Wall a)
            {
                float closestX = Math.Max(a.X, Math.Min(other.X, a.X + a.Width));
                float closestY = Math.Max(a.Y, Math.Min(other.Y, a.Y + a.Height));

                float dx = other.X - closestX;
                float dy = other.Y - closestY;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance >= other.Radius) return;

                float overlap = other.Radius - distance;

                float nx = dx / distance;
                float ny = dy / distance;

                float totalMass = a.Mass + other.Mass;
                float otherMoveRatio = a.Mass / totalMass;

                other.X += nx * overlap * otherMoveRatio;
                other.Y += ny * overlap * otherMoveRatio;

                float dotProductOther = other.VelocityX * nx + other.VelocityY * ny;

                other.VelocityX -= 2 * dotProductOther * nx;
                other.VelocityY -= 2 * dotProductOther * ny;

                other.VelocityX *= BounceFactor;
                other.VelocityY *= BounceFactor;
            }
        }
    }
}
