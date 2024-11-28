using AirHockey.Ambience.Effects;
using AirHockey.Ambience.Iterators;

namespace AirHockey.Ambience.Collections
{
    public class LightingEffectCollection : IAggregate<LightingEffect>
    {
        private readonly List<LightingEffect> _effects = new List<LightingEffect>();

        public void AddEffect(LightingEffect effect) => _effects.Add(effect);

        public IIterator<LightingEffect> CreateIterator() => new LightingEffectIterator(_effects);
    }
}
