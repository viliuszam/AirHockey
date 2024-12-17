namespace AirHockey.Actors.Powerups.PowerupDecorators
{
    using System.Diagnostics.CodeAnalysis;
    using System.Timers;

    public class AccelerationMultiplierDecorator : PowerupDecorator
    {
        private readonly float _accelerationMultiplier;
        private readonly float _duration;
        private readonly Timer _timer;
        private float _originalAcceleration;

        public AccelerationMultiplierDecorator(Powerup powerup, float accelerationMultiplier, float duration)
            : this(powerup, accelerationMultiplier, duration, new Timer(duration * 1000)) { }

        // Overloaded constructor for injecting a Timer (for unit tests)
        public AccelerationMultiplierDecorator(Powerup powerup, float accelerationMultiplier, float duration, Timer timer)
            : base(powerup)
        {
            _accelerationMultiplier = accelerationMultiplier;
            _duration = duration;
            _timer = timer;
        }

        public void Activate(Player player)
        {
            _originalAcceleration = player.Acceleration;
            WrappedPowerup.Activate(player);
            player.Acceleration *= _accelerationMultiplier;

            Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} activated with acceleration multiplier! New Acceleration: {player.Acceleration}");

            _timer.Elapsed += (sender, e) => ResetAcceleration(player);
            _timer.Start();
        }

        // Separate method to reset acceleration, making it testable
        public void ResetAcceleration(Player player)
        {
            player.Acceleration = _originalAcceleration;
            _timer.Stop();
            _timer.Dispose();
            Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} acceleration effect ended. Acceleration reverted to: {player.Acceleration}");
        }

        public override Powerup CloneShallow()
        {
            return new AccelerationMultiplierDecorator(WrappedPowerup.CloneShallow(), _accelerationMultiplier, _duration);
        }

        public override Powerup CloneDeep()
        {
            return new AccelerationMultiplierDecorator(WrappedPowerup.CloneDeep(), _accelerationMultiplier, _duration);
        }

        [ExcludeFromCodeCoverage]
        public override void Update()
        {
        }
    }
}
