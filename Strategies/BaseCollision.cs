using AirHockey.Actors;

namespace AirHockey.Strategies
{
    public class BaseCollision : ICollision
    {
        public void ResolveCollision(Entity a, Entity b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance == 0) return;

            float overlap = (a.Radius + b.Radius) - distance;

            float totalMass = a.Mass + b.Mass;
            float moveX = (overlap * dx) / distance;
            float moveY = (overlap * dy) / distance;

            a.X -= moveX * (b.Mass / totalMass);
            a.Y -= moveY * (b.Mass / totalMass);
            b.X += moveX * (a.Mass / totalMass);
            b.Y += moveY * (a.Mass / totalMass);

            float nx = dx / distance;
            float ny = dy / distance;

            float kx = a.VelocityX - b.VelocityX;
            float ky = a.VelocityY - b.VelocityY;

            float p = 2.0f * (nx * kx + ny * ky) / totalMass;

            a.VelocityX -= p * b.Mass * nx;
            a.VelocityY -= p * b.Mass * ny;
            b.VelocityX += p * a.Mass * nx;
            b.VelocityY += p * a.Mass * ny;

            float energyFactor = 1.1f;
            a.VelocityX *= energyFactor;
            a.VelocityY *= energyFactor;
            b.VelocityX *= energyFactor;
            b.VelocityY *= energyFactor;
        }
    }
}
