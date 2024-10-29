using System.Diagnostics.CodeAnalysis;

namespace AirHockey.Actors.Powerups
{
    public class DashPowerup : Powerup
    {
        public float DashSpeed;
        public float DashDuration;
        public DashPowerup(float x, float y, int id, float speed = 8f, float duration = 0.25f) : base(x, y, id)
        {
            DashSpeed = speed;
            DashDuration = duration;
        }

        public override void Activate(Player player)
        {
            var prevSpeed = player.MaxSpeed;
            player.MaxSpeed = DashSpeed;
            player.VelocityX *= DashSpeed;
            player.VelocityY *= DashSpeed;
            System.Timers.Timer timer = new(DashDuration * 1000);
            timer.Elapsed += (sender, e) =>
            {
                player.MaxSpeed = prevSpeed;
                timer.Stop();
            };
            timer.Start();
        }

        public override Powerup CloneDeep()
        {
            return new DashPowerup(this.X, this.Y, this.Id, this.DashSpeed, this.DashDuration);
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
