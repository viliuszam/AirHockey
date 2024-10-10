namespace AirHockey.Actors.Walls
{
    public class ScrollingWall : Wall
    {
        private const float scrollSpeed = 0.01f;
        private string direction = "UP";
        private int currentIter = 0;
        private int iterCount = 1000;

        public ScrollingWall(int id, float width, float height)
            : base(id, width, height)
        {
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
                        X -= overlapX / 2;
                    }
                    else
                    {
                        if(other.Acceleration != 0f){
                            other.X -= overlapX / 2;
                        }
                        X += overlapX / 2;
                    }

                    if(other.Acceleration != 0f){
                        other.VelocityX = -(other.VelocityX);
                    }

                    VelocityX = -VelocityX;
                }
                else
                {
                    if (deltaY > 0)
                    {
                        if(other.Acceleration != 0f){
                            other.Y += overlapY / 2;
                        }
                        Y -= overlapY / 2;
                    }
                    else
                    {
                        if(other.Acceleration != 0f){
                            other.Y -= overlapY / 2;
                        }
                        Y += overlapY / 2;
                    }

                    if(other.Acceleration != 0f){
                        other.VelocityX = -(other.VelocityX);
                    }

                    VelocityY = -VelocityY;
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

                X -= nx * overlap * wallMoveRatio;
                Y -= ny * overlap * wallMoveRatio;

                float velocityTransferRatio = other.Mass / totalMass;

                VelocityX += -other.VelocityX * velocityTransferRatio;
                VelocityY += -other.VelocityY * velocityTransferRatio;
            }
        }

        public override void Update(){
            MoveUpDown();
            ApplyFriction();
            Move();
        }

        public void MoveUpDown(){
            if(direction == "UP"){
                VelocityY += scrollSpeed;
                currentIter++;
                if(currentIter == iterCount){
                    direction = "DOWN";
                    currentIter = 0;
                }
            }
            else if (direction == "DOWN"){
                VelocityY -= scrollSpeed;
                currentIter++;
                if(currentIter == iterCount){
                    direction = "UP";
                    currentIter = 0;
                }
            }
        }
    }
}