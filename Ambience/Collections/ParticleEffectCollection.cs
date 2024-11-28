using AirHockey.Ambience.Effects;
using AirHockey.Ambience.Iterators;

namespace AirHockey.Ambience.Collections
{
    public class ParticleEffectCollection : IAggregate<ParticleEffect>
    {
        private HashSet<ParticleEffect> _effects = new HashSet<ParticleEffect>();

        public void AddEffect(ParticleEffect effect) => _effects.Add(effect);

        public IIterator<ParticleEffect> CreateIterator() => new ParticleEffectIterator(ref _effects);
    }
}
