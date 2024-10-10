namespace AirHockey.Actors.Walls
{
    public class StandardWall : Wall
    {
        private const float BounceFactor = 0.2f;
        private bool Movable;
        public StandardWall(int id, float width, float height, bool moveable)
            : base(id, width, height)
        {
            Movable = moveable;
        }

        public override void ResolveCollision(Entity other)
        {
            float closestX = Math.Max(X, Math.Min(other.X, X + Width));
            float closestY = Math.Max(Y, Math.Min(other.Y, Y + Height));

            float dx = other.X - closestX;
            float dy = other.Y - closestY;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance >= other.Radius) return;

            float overlap = other.Radius - distance;

            float nx = dx / distance;
            float ny = dy / distance;

            float totalMass = Mass + other.Mass;
            float otherMoveRatio = Mass / totalMass;

            other.X += nx * overlap * otherMoveRatio;
            other.Y += ny * overlap * otherMoveRatio;

            float dotProductOther = other.VelocityX * nx + other.VelocityY * ny;

            other.VelocityX -= 2 * dotProductOther * nx;
            other.VelocityY -= 2 * dotProductOther * ny;

            other.VelocityX *= BounceFactor;
            other.VelocityY *= BounceFactor;
        }
        

        public override void Update(){}

    }
}