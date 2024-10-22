using AirHockey.Actors;

namespace AirHockey.Effects.Behaviors
{
    public class LowGravityBehavior : IEffectBehavior
    {
        public void Execute(Entity entity)
        {
            entity.Mass *= 0.5f;
        }

        public void Revert(Entity entity)
        {
            entity.Mass *= 2f;
        }

        public string Identifier()
        {
            return "LOW_GRAVITY";
        }
    }
}
