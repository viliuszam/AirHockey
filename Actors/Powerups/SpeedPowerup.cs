namespace AirHockey.Actors.Powerups
{
    public class SpeedPowerup : Powerup
    {
        private readonly float speedMultiplier;
        private readonly float duration;

        public SpeedPowerup(float x, float y, float multiplier = 2f, float duration = 5f)
            : base(x, y)
        {
            speedMultiplier = multiplier;
            this.duration = duration;
        }

        public override void Activate(Player player)
        {
            // Bazine logika speed powerupui, turbut reiks tobulint

            player.MaxSpeed *= speedMultiplier;

            System.Timers.Timer timer = new System.Timers.Timer(duration * 1000);
            timer.Elapsed += (sender, e) =>
            {
                player.MaxSpeed /= speedMultiplier;
                timer.Stop();
            };
            timer.Start();
        }
    }
}
