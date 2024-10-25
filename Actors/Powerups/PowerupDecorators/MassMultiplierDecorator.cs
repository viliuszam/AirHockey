namespace AirHockey.Actors.Powerups.PowerupDecorators
{
    using System.Timers;

    public class MassMultiplierDecorator : PowerupDecorator
    {
        private readonly float _massMultiplier;
        private readonly float _duration;

        public MassMultiplierDecorator(Powerup powerup, float massMultiplier, float duration) : base(powerup)
        {
            _massMultiplier = massMultiplier;
            _duration = duration;
        }

        public override void Activate(Player player)
        {
            float originalMass = player.Mass;

            WrappedPowerup.Activate(player);

            player.Mass *= _massMultiplier;

            Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} activated with mass multiplier! New Mass: {player.Mass}");

            Timer timer = new Timer(_duration * 1000);
            timer.Elapsed += (sender, e) =>
            {
                player.Mass = originalMass;
                timer.Stop();
                timer.Dispose();
                Console.WriteLine($"{WrappedPowerup.GetBaseType().Name} mass effect ended. Mass reverted to: {player.Mass}");
            };
            timer.Start();
        }

        public override Powerup CloneShallow()
        {
            return new MassMultiplierDecorator(WrappedPowerup.CloneShallow(), _massMultiplier, _duration);
        }

        public override Powerup CloneDeep()
        {
            return new MassMultiplierDecorator(WrappedPowerup.CloneDeep(), _massMultiplier, _duration);
        }

        public override void Update()
        {
            
        }
    }
}
