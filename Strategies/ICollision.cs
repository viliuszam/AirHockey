using AirHockey.Actors;

namespace AirHockey.Strategies
{
    public interface ICollision
    {
        public void ResolveCollision(Entity entity1, Entity entity2);
    }

}
