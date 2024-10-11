namespace AirHockey.Actors.Powerups
{
    public class PowerupFactory
    {
        public Powerup CreatePowerup(float x, float y, string type)
        {
            switch (type)
            {
                case "Dash":
                    return new DashPowerup(x, y);
                case "Freeze":
                    return new FreezePowerup(x, y);
                case "Projectile":
                    return new ProjectilePowerup(x, y);
                default:
                    throw new ArgumentException("Invalid powerup type");
            }
        }
    }
}
