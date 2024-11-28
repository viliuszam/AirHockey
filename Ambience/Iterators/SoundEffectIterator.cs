using AirHockey.Ambience.Effects;

namespace AirHockey.Ambience.Iterators
{
    public class SoundEffectIterator : IIterator<SoundEffect>
    {
        private readonly Queue<SoundEffect> _effects;

        public SoundEffectIterator(Queue<SoundEffect> effects)
        {
            _effects = effects;
        }

        public bool HasNext() => _effects.Count > 0;

        public SoundEffect Next()
        {
            if (!HasNext()) throw new InvalidOperationException();
            return _effects.Dequeue();
        }

        public SoundEffect First()
        {
            return _effects.Count > 0 ? _effects.Peek() : null;
        }

        public void Add(SoundEffect item) => _effects.Enqueue(item);

        public void Remove(SoundEffect item)
        {
            throw new NotSupportedException("Sound Queue has no removal, this shouldn't be called.");
        }
    }
}
