using System.Drawing;
using System.Numerics;

namespace AirHockey.Ambience.Effects
{
    public class LightingEffect
    {
        public Rectangle Area { get; set; }
        public float Duration { get; set; }
        public Color Color { get; set; }

        private float _elapsedTime;

        public LightingEffect(Rectangle area, float duration, Color color)
        {
            Area = area;
            Duration = duration;
            Color = color;
            _elapsedTime = 0;
        }

        public void Update(float deltaTime)
        {
            _elapsedTime += deltaTime;
        }

        public bool IsComplete() => _elapsedTime >= Duration;

    }
}
