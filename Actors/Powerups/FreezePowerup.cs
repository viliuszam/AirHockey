namespace AirHockey.Actors.Powerups
{
    public class FreezePowerup : Powerup
    {
        public float FreezeDuration;
        public FreezePowerup(float x, float y, int id, float freezeDuration = 10f) : base(x, y, id)
        {
            FreezeDuration = freezeDuration;
        }

        public override void Activate(Player player)
        {
            var enemyPlayer = player.Id == player.Room.Players[0].Id ? player.Room.Players[1] : player.Room.Players[0];
            var prevSpeed = enemyPlayer.MaxSpeed;
            enemyPlayer.MaxSpeed = 0;
            enemyPlayer.VelocityX *= 0;
            enemyPlayer.VelocityY *= 0;
            System.Timers.Timer timer = new(FreezeDuration * 1000);
            timer.Elapsed += (sender, e) =>
            {
                enemyPlayer.MaxSpeed = prevSpeed;
                timer.Stop();
            };
            timer.Start();
        }

        public override Powerup CloneDeep()
        {
            return new FreezePowerup(this.X, this.Y, this.Id, this.FreezeDuration);
        }

        public override Powerup CloneShallow()
        {
            return (FreezePowerup)this.MemberwiseClone();
        }

        public override void Update()
        {

        }
    }
}
