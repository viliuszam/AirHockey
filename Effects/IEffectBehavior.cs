using AirHockey.Actors;

namespace AirHockey.Effects
{
    public interface IEffectBehavior
    {
        void Execute(Entity entity);
        void Revert(Entity entity);

        string Identifier();
    }
}
