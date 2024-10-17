using AirHockey.Actors;
using AirHockey.Actors.Walls;

namespace AirHockey.Strategies
{
    public class QuickCollision:ICollision
    {
        public void ResolveCollision(Entity entity, Entity other)
        {
            if (entity is QuickSandWall a)
            {
                other.VelocityX = other.VelocityX * a.GetSlowFactor();
                other.VelocityY = other.VelocityY * a.GetSlowFactor();
            }
        }
    }
}
