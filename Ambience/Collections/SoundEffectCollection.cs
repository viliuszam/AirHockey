using AirHockey.Ambience.Effects;
using AirHockey.Ambience.Iterators;

namespace AirHockey.Ambience.Collections
{
    public class SoundEffectCollection : IAggregate<SoundEffect>
    {
        private Queue<SoundEffect> _effects = new Queue<SoundEffect>();

        public void AddEffect(SoundEffect effect) => _effects.Enqueue(effect);

        public IIterator<SoundEffect> CreateIterator() => new SoundEffectIterator(ref _effects);
    }
}
