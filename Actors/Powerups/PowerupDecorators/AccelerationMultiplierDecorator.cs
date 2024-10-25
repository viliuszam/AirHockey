namespace AirHockey.Actors.Powerups.PowerupDecorators
{
    using System.Timers;

    public class AccelerationMultiplierDecorator : PowerupDecorator
    {
        private readonly float _accelerationMultiplier;
        private readonly float _duration;

        public AccelerationMultiplierDecorator(Powerup powerup, float accelerationMultiplier, float duration) : base(powerup)
        {
            _accelerationMultiplier = accelerationMultiplier;
            _duration = duration;
        }

        public override void Activate(Player player)
        {
            float originalAcceleration = player.Acceleration;

            WrappedPowerup.Activate(player);

            player.Acceleration *= _accelerationMultiplier;

            Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} activated with acceleration multiplier! New Acceleration: {player.Acceleration}");

            Timer timer = new Timer(_duration * 1000);
            timer.Elapsed += (sender, e) =>
            {
                player.Acceleration = originalAcceleration;
                timer.Stop();
                timer.Dispose();
                Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} acceleration effect ended. Acceleration reverted to: {player.Acceleration}");
            };
            timer.Start();
        }

        public override Powerup CloneShallow()
        {
            return new AccelerationMultiplierDecorator(WrappedPowerup.CloneShallow(), _accelerationMultiplier, _duration);
        }

        public override Powerup CloneDeep()
        {
            return new AccelerationMultiplierDecorator(WrappedPowerup.CloneDeep(), _accelerationMultiplier, _duration);
        }

        public override void Update()
        {
            
        }
    }
}
