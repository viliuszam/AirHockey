using AirHockey.Actors;

namespace AirHockey.Effects
{
    public abstract class EnvironmentalEffect
    {
        public int ID { get; private set; }

        protected IEffectBehavior _behavior;
        public float Duration { get; set; }

        public EnvironmentalEffect(int id, IEffectBehavior behavior, float duration)
        {
            ID = id;
            _behavior = behavior;
            Duration = duration;
        }

        public abstract void ApplyEffect(Room room);
        public abstract void RemoveEffect(Room room);

        public IEffectBehavior GetBehavior() { return _behavior; }

        }
}
