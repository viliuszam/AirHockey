using AirHockey.Actors;

namespace AirHockey.Effects.Behaviors
{
    public class WindBehavior : IEffectBehavior
    {
        private float windStrength = 0.5f;
        private float windAngle;

        public WindBehavior()
        {
            windAngle = (float)(new Random().NextDouble() * 2 * Math.PI);
        }

        public void Execute(Entity entity)
        {
            entity.VelocityX += (float)Math.Cos(windAngle) * windStrength;
            entity.VelocityY += (float)Math.Sin(windAngle) * windStrength;
        }

        public void Revert(Entity entity)
        {
        }

        public string Identifier()
        {
            return $"WIND,{windAngle}";
        }
    }
}
