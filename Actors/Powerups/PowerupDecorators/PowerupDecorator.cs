using System.Diagnostics.CodeAnalysis;

namespace AirHockey.Actors.Powerups.PowerupDecorators
{
    public abstract class PowerupDecorator : Powerup
    {
        protected Powerup WrappedPowerup;

        protected PowerupDecorator(Powerup powerup) : base(powerup.X, powerup.Y, powerup.Id)
        {
            WrappedPowerup = powerup;
        }

        //note: sitie excludinami nes coverinami implementacijose

        [ExcludeFromCodeCoverage]
        public void Activate(Player player)
        {
            WrappedPowerup.Activate(player);
        }

        [ExcludeFromCodeCoverage]
        public override Type GetBaseType()
        {
            return WrappedPowerup.GetBaseType();
        }

        [ExcludeFromCodeCoverage]
        public override Powerup CloneShallow()
        {
            return WrappedPowerup.CloneShallow();
        }

        [ExcludeFromCodeCoverage]
        public override Powerup CloneDeep()
        {
            return WrappedPowerup.CloneDeep();
        }

    }
}
