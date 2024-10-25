namespace AirHockey.Actors.Powerups.PowerupDecorators
{
    public abstract class PowerupDecorator : Powerup
    {
        protected Powerup WrappedPowerup;

        protected PowerupDecorator(Powerup powerup) : base(powerup.X, powerup.Y, powerup.Id)
        {
            WrappedPowerup = powerup;
        }

        public override void Activate(Player player)
        {
            WrappedPowerup.Activate(player);
        }

        public override Type GetBaseType()
        {
            return WrappedPowerup.GetBaseType();
        }
        public override Powerup CloneShallow()
        {
            return WrappedPowerup.CloneShallow();
        }

        public override Powerup CloneDeep()
        {
            return WrappedPowerup.CloneDeep();
        }

    }
}
