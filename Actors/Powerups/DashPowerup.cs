using System.Diagnostics.CodeAnalysis;

namespace AirHockey.Actors.Powerups
{
    public class DashPowerup : Powerup
    {
        private float _prevSpeed;
        public readonly float DashSpeed;

        public DashPowerup(float x, float y, int id, float speed = 8f, float duration = 0.25f) : base(x, y, id)
        {
            DashSpeed = speed;
            Duration = duration;
        }

        protected override void ApplyEffect(Player player)
        {
            _prevSpeed = player.MaxSpeed;
            player.MaxSpeed = DashSpeed;
            player.VelocityX *= DashSpeed;
            player.VelocityY *= DashSpeed;
        }

        protected override void RemoveEffect(Player player)
        {
            player.MaxSpeed = _prevSpeed;
        }

        public override Powerup CloneDeep()
        {
            return new DashPowerup(this.X, this.Y, this.Id, this.DashSpeed, this.Duration);
        }

        public override Powerup CloneShallow()
        {
            return (DashPowerup)this.MemberwiseClone();
        }

        [ExcludeFromCodeCoverage]
        public override void Update()
        {

        }
    }
}
