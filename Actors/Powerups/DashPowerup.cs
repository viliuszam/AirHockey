namespace AirHockey.Actors.Powerups
{
    public class DashPowerup : Powerup
    {
        private readonly float _dashSpeed;
        private readonly float _dashDuration;
        public DashPowerup(float x, float y, int id, float speed = 8f, float duration = 0.25f) : base(x, y, id)
        {
            _dashSpeed = speed;
            _dashDuration = duration;
        }

        public override void Activate(Player player)
        {
            var prevSpeed = player.MaxSpeed;
            player.MaxSpeed = _dashSpeed;
            player.VelocityX *= _dashSpeed;
            player.VelocityY *= _dashSpeed;
            System.Timers.Timer timer = new(_dashDuration * 1000);
            timer.Elapsed += (sender, e) =>
            {
                player.MaxSpeed = prevSpeed;
                timer.Stop();
            };
            timer.Start();
        }

        public override void Update()
        {

        }
    }
}
