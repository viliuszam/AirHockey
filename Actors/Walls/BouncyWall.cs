namespace AirHockey.Actors.Walls
{
    public class BouncyWall : Wall
    {
        private const float BounceFactor = 0.99f;
        private bool Movable;

        public BouncyWall(int id, float width, float height, bool moveable)
            : base(id, width, height)
        {
            Movable = moveable;

        }
        public override void ResolveCollision(Entity other)
        {
            if (other is Wall wall)
            {
                float deltaX = wall.X + wall.Width / 2 - (X + Width / 2);
                float deltaY = wall.Y + wall.Height / 2 - (Y + Height / 2);

                float overlapX = (Width / 2 + wall.Width / 2) - Math.Abs(deltaX);
                float overlapY = (Height / 2 + wall.Height / 2) - Math.Abs(deltaY);

                if (overlapX <= 0 || overlapY <= 0) return;

                if (overlapX < overlapY)
                {
                    if (deltaX > 0)
                    {
                        if(other.Acceleration != 0f){
                            other.X += overlapX / 2;
                        }
                        if (Movable) X -= overlapX / 2;
                    }
                    else
                    {
                        if(other.Acceleration != 0f){
                            other.X -= overlapX / 2;
                        }
                        if (Movable) X += overlapX / 2;
                    }

                    if(other.Acceleration != 0f){
                        other.VelocityX = -(other.VelocityX * BounceFactor);
                    }

                    if (Movable)
                    {
                        VelocityX = -VelocityX * BounceFactor;
                    }
                }
                else
                {
                    if (deltaY > 0)
                    {
                        if(other.Acceleration != 0f){
                            other.Y += overlapY / 2;
                        }
                        if (Movable) Y -= overlapY / 2;
                    }
                    else
                    {
                        if(other.Acceleration != 0f){
                            other.Y -= overlapY / 2;
                        }
                        if (Movable) Y += overlapY / 2;
                    }

                    if(other.Acceleration != 0f){
                        other.VelocityX = -(other.VelocityX * BounceFactor);
                    }

                    if (Movable)
                    {
                        VelocityY = -VelocityY * BounceFactor;
                    }
                }
            }
            else
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
                float wallMoveRatio = other.Mass / totalMass;

                other.X += nx * overlap * otherMoveRatio;
                other.Y += ny * overlap * otherMoveRatio;

                float dotProductOther = other.VelocityX * nx + other.VelocityY * ny;
                other.VelocityX -= 2 * dotProductOther * nx;
                other.VelocityY -= 2 * dotProductOther * ny;

                other.VelocityX *= BounceFactor;
                other.VelocityY *= BounceFactor;

                if (Movable)
                {
                    X -= nx * overlap * wallMoveRatio;
                    Y -= ny * overlap * wallMoveRatio;

                    float velocityTransferRatio = other.Mass / totalMass;

                    VelocityX += -other.VelocityX * velocityTransferRatio;
                    VelocityY += -other.VelocityY * velocityTransferRatio;

                    VelocityX *= BounceFactor;
                    VelocityY *= BounceFactor;
                }
            }
        }

        public override void Update(){
            if(Movable){
                ApplyFriction();
                Move();
            }
        }

    }
}