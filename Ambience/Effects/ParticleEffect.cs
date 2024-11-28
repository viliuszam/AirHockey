using System.Numerics;

namespace AirHockey.Ambience.Effects
{
    public enum ParticleType
    {
        Spark,
        Explosion,
        Trail
    }

    public class ParticleEffect
    {
        public ParticleType Type { get; set; }
        public Vector2 Position { get; set; }
        public float Lifetime { get; set; }
        public int ParticleCount { get; set; }

        private float _elapsedTime;

        public ParticleEffect(ParticleType type, Vector2 position, float lifetime, int particleCount)
        {
            Type = type;
            Position = position;
            Lifetime = lifetime;
            ParticleCount = particleCount;
            _elapsedTime = 0;
        }

        public void Update(float deltaTime)
        {
            _elapsedTime += deltaTime;
        }

        public bool IsActive() => _elapsedTime < Lifetime;
    }
}
