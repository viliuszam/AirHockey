using AirHockey.Ambience.Effects;

namespace AirHockey.Ambience.Iterators
{
    public class ParticleEffectIterator : IIterator<ParticleEffect>
    {
        private readonly HashSet<ParticleEffect> _effects;
        private IEnumerator<ParticleEffect> _enumerator;

        public ParticleEffectIterator(HashSet<ParticleEffect> effects)
        {
            _effects = effects;
            _enumerator = _effects.GetEnumerator();
        }

        public bool HasNext() => _enumerator.MoveNext();

        public ParticleEffect Next()
        {
            if (!HasNext()) throw new InvalidOperationException();
            return _enumerator.Current;
        }

        public ParticleEffect First()
        {
            _enumerator = _effects.GetEnumerator();
            return _enumerator.MoveNext() ? _enumerator.Current : null;
        }

        public void Add(ParticleEffect item) => _effects.Add(item);

        public void Remove(ParticleEffect item) => _effects.Remove(item);
    }
}
