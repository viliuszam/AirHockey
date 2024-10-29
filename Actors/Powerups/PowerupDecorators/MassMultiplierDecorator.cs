namespace AirHockey.Actors.Powerups.PowerupDecorators
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Timers;

    public class MassMultiplierDecorator : PowerupDecorator
    {
        private readonly float _massMultiplier;
        private readonly float _duration;
        private readonly Timer _timer;
        private float _originalMass;

        public MassMultiplierDecorator(Powerup powerup, float massMultiplier, float duration)
            : this(powerup, massMultiplier, duration, new Timer(duration * 1000)) { }

        public MassMultiplierDecorator(Powerup powerup, float massMultiplier, float duration, Timer timer)
            : base(powerup)
        {
            _massMultiplier = massMultiplier;
            _duration = duration;
            _timer = timer;
        }

        public override void Activate(Player player)
        {
            _originalMass = player.Mass;
            WrappedPowerup.Activate(player);
            player.Mass *= _massMultiplier;

            Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} activated with mass multiplier! New Mass: {player.Mass}");

            _timer.Elapsed += (sender, e) => ResetMass(player);
            _timer.Start();
        }

        // Separate reset method for testability
        public void ResetMass(Player player)
        {
            player.Mass = _originalMass;
            _timer.Stop();
            _timer.Dispose();
            Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} mass effect ended. Mass reverted to: {player.Mass}");
        }

        [ExcludeFromCodeCoverage]
        public override Powerup CloneShallow()
        {
            return new MassMultiplierDecorator(WrappedPowerup.CloneShallow(), _massMultiplier, _duration);
        }

        [ExcludeFromCodeCoverage]
        public override Powerup CloneDeep()
        {
            return new MassMultiplierDecorator(WrappedPowerup.CloneDeep(), _massMultiplier, _duration);
        }

        [ExcludeFromCodeCoverage]
        public override void Update()
        {
        }
    }
}
