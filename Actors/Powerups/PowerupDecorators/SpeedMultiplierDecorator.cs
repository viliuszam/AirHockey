namespace AirHockey.Actors.Powerups.PowerupDecorators
{
    using System.Timers;
    public class SpeedMultiplierDecorator : PowerupDecorator
    {
        private readonly float _speedMultiplier;
        private readonly float _duration;

        public SpeedMultiplierDecorator(Powerup powerup, float speedMultiplier, float duration) : base(powerup)
        {
            _speedMultiplier = speedMultiplier;
            _duration = duration;
        }

        public override void Activate(Player player)
        {
            float originalSpeed = player.MaxSpeed;

            WrappedPowerup.Activate(player);

            player.MaxSpeed *= _speedMultiplier;

            Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} activated with speed multiplier! New Speed: {player.MaxSpeed}");

            Timer timer = new Timer(_duration * 1000);
            timer.Elapsed += (sender, e) =>
            {
                player.MaxSpeed = originalSpeed;
                timer.Stop();
                timer.Dispose();
                Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} speed effect ended. Speed reverted to: {player.MaxSpeed}");
            };
            timer.Start();
        }

        public override Powerup CloneShallow()
        {
            return new SpeedMultiplierDecorator(WrappedPowerup.CloneShallow(), _speedMultiplier, _duration);
        }

        public override Powerup CloneDeep()
        {
            return new SpeedMultiplierDecorator(WrappedPowerup.CloneDeep(), _speedMultiplier, _duration);
        }

        public override void Update()
        {
            
        }
    }
}
