using AirHockey.Actors.Walls;
using AirHockey.Actors;

namespace AirHockey.Strategies
{
    public class BouncyCollision: ICollision
    {
        public void ResolveCollision(Entity entity, Entity other)
        {
            if (entity is BouncyWall a)
            {

                if (other is Wall wall)
                {
                    float deltaX = wall.X + wall.Width / 2 - (a.X + a.Width / 2);
                    float deltaY = wall.Y + wall.Height / 2 - (a.Y + a.Height / 2);

                    float overlapX = (a.Width / 2 + wall.Width / 2) - Math.Abs(deltaX);
                    float overlapY = (a.Height / 2 + wall.Height / 2) - Math.Abs(deltaY);

                    if (overlapX <= 0 || overlapY <= 0) return;

                    if (overlapX < overlapY)
                    {
                        if (deltaX > 0)
                        {
                            if (other.Acceleration != 0f)
                            {
                                other.X += overlapX / 2;
                            }
                            if (a.getMove()) a.X -= overlapX / 2;
                        }
                        else
                        {
                            if (other.Acceleration != 0f)
                            {
                                other.X -= overlapX / 2;
                            }
                            if (a.getMove()) a.X += overlapX / 2;
                        }

                        if (other.Acceleration != 0f)
                        {
                            other.VelocityX = -(other.VelocityX * a.GetBounce());
                        }

                        if (a.getMove())
                        {
                            a.VelocityX = -a.VelocityX * a.GetBounce();
                        }
                    }
                    else
                    {
                        if (deltaY > 0)
                        {
                            if (other.Acceleration != 0f)
                            {
                                other.Y += overlapY / 2;
                            }
                            if (a.getMove()) a.Y -= overlapY / 2;
                        }
                        else
                        {
                            if (other.Acceleration != 0f)
                            {
                                other.Y -= overlapY / 2;
                            }
                            if (a.getMove()) a.Y += overlapY / 2;
                        }

                        if (other.Acceleration != 0f)
                        {
                            other.VelocityX = -(other.VelocityX * a.GetBounce());
                        }

                        if (a.getMove())
                        {
                            a.VelocityY = -a.VelocityY * a.GetBounce();
                        }
                    }
                }
                else
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
                    float wallMoveRatio = other.Mass / totalMass;

                    other.X += nx * overlap * otherMoveRatio;
                    other.Y += ny * overlap * otherMoveRatio;

                    float dotProductOther = other.VelocityX * nx + other.VelocityY * ny;
                    other.VelocityX -= 2 * dotProductOther * nx;
                    other.VelocityY -= 2 * dotProductOther * ny;

                    other.VelocityX *= a.GetBounce();
                    other.VelocityY *= a.GetBounce();
                }
            }
        }
    }
}
