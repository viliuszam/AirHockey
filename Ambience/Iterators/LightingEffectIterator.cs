using System.Collections;
using AirHockey.Ambience.Effects;

namespace AirHockey.Ambience.Iterators
{
    public class LightingEffectIterator : IIterator<LightingEffect>
    {
        private readonly List<LightingEffect> _effects;
        private int _position;

        public LightingEffectIterator(ref List<LightingEffect> effects)
        {
            _effects = effects;
            _position = 0;
        }

        public bool HasNext() => _position < _effects.Count;

        public LightingEffect Next()
        {
            if (!HasNext()) throw new InvalidOperationException();
            return _effects[_position++];
        }

        public LightingEffect First()
        {
            _position = 0;
            return _effects.Count > 0 ? _effects[0] : null;
        }

        public void Add(LightingEffect item) => _effects.Add(item);

        public void Remove(LightingEffect item) => _effects.Remove(item);

        public IEnumerator<LightingEffect> GetEnumerator() => _effects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
