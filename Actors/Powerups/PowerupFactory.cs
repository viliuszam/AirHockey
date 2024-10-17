namespace AirHockey.Actors.Powerups
{
    public class PowerupFactory
    {
        public Powerup CreatePowerup(float x, float y, int id, string type)
        {
            switch (type)
            {
                case "Dash":
                    return new DashPowerup(x, y, id);
                case "Freeze":
                    return new FreezePowerup(x, y, id);
                case "Projectile":
                    return new ProjectilePowerup(x, y, id);
                default:
                    throw new ArgumentException("Invalid powerup type");
            }
        }
    }
}
