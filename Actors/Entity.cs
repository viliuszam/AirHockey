using System;

namespace AirHockey.Actors
{
    public abstract class Entity
    {
        private float _x;
        private float _y;
        private float _velocityX;
        private float _velocityY;

        public float X
        {
            get => _x;
            set => _x = float.IsFinite(value) ? value : 0f;
        }

        public float Y
        {
            get => _y;
            set => _y = float.IsFinite(value) ? value : 0f;
        }

        public float VelocityX
        {
            get => _velocityX;
            set => _velocityX = float.IsFinite(value) ? value : 0f;
        }

        public float VelocityY
        {
            get => _velocityY;
            set => _velocityY = float.IsFinite(value) ? value : 0f;
        }

        public float Acceleration { get; set; }
        public float Friction { get; set; } = 0.99f;
        public float Radius { get; set; }
        public float Mass { get; set; }

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

        public bool IsColliding(Entity other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            return distance < (Radius + other.Radius);
        }

        public void ResolveCollision(Entity other)
        {
            // elastiniu susidurimu kopypasta tiesiai is stackoverflow

            float dx = other.X - X;
            float dy = other.Y - Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance == 0) return;

            float overlap = (Radius + other.Radius) - distance;

            float totalMass = Mass + other.Mass;
            float moveX = (overlap * dx) / distance;
            float moveY = (overlap * dy) / distance;

            X -= moveX * (other.Mass / totalMass);
            Y -= moveY * (other.Mass / totalMass);
            other.X += moveX * (Mass / totalMass);
            other.Y += moveY * (Mass / totalMass);

            float nx = dx / distance;
            float ny = dy / distance;

            float kx = VelocityX - other.VelocityX;
            float ky = VelocityY - other.VelocityY;
            float p = 2.0f * (nx * kx + ny * ky) / (Mass + other.Mass);

            VelocityX -= p * other.Mass * nx;
            VelocityY -= p * other.Mass * ny;
            other.VelocityX += p * Mass * nx;
            other.VelocityY += p * Mass * ny;

            float energyFactor = 1.1f;
            VelocityX *= energyFactor;
            VelocityY *= energyFactor;
            other.VelocityX *= energyFactor;
            other.VelocityY *= energyFactor;
        }

        public void ConstrainToBounds(float minX, float minY, float maxX, float maxY)
        {
            if (X - Radius < minX)
            {
                X = minX + Radius;
                VelocityX = -VelocityX * 0.8f;
            }
            else if (X + Radius > maxX)
            {
                X = maxX - Radius;
                VelocityX = -VelocityX * 0.8f;
            }

            if (Y - Radius < minY)
            {
                Y = minY + Radius;
                VelocityY = -VelocityY * 0.8f;
            }
            else if (Y + Radius > maxY)
            {
                Y = maxY - Radius;
                VelocityY = -VelocityY * 0.8f;
            }
        }
    }
}