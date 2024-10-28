namespace AirHockey.Actors.Powerups.PowerupDecorators
{
    using System;
    using System.Timers;

    public class SpeedMultiplierDecorator : PowerupDecorator
    {
        private readonly float _speedMultiplier;
        private readonly float _duration;
        private readonly Timer _timer;
        private float _originalSpeed;

        public SpeedMultiplierDecorator(Powerup powerup, float speedMultiplier, float duration)
            : this(powerup, speedMultiplier, duration, new Timer(duration * 1000)) { }

        public SpeedMultiplierDecorator(Powerup powerup, float speedMultiplier, float duration, Timer timer)
            : base(powerup)
        {
            _speedMultiplier = speedMultiplier;
            _duration = duration;
            _timer = timer;
        }

        public override void Activate(Player player)
        {
            _originalSpeed = player.MaxSpeed;
            WrappedPowerup.Activate(player);
            player.MaxSpeed *= _speedMultiplier;

            Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} activated with speed multiplier! New Speed: {player.MaxSpeed}");

            _timer.Elapsed += (sender, e) => ResetSpeed(player);
            _timer.Start();
        }

        // Separate reset method for testability
        public void ResetSpeed(Player player)
        {
            player.MaxSpeed = _originalSpeed;
            _timer.Stop();
            _timer.Dispose();
            Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} speed effect ended. Speed reverted to: {player.MaxSpeed}");
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
            // Additional update logic if necessary
        }
    }
}
