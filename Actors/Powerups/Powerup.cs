namespace AirHockey.Actors.Powerups
{
    public abstract class Powerup : Entity
    {
        public int Id { get; set; }
        public bool IsActive { get; private set; }

        public Powerup(float x, float y, int id)
        {
            X = x;
            Y = y;
            Radius = 20f;
            IsActive = true;
            Id = id;
        }

        public void ResolveCollision(Player other)
        {
            IsActive = false;
            other.ActivePowerup = this;
            Console.WriteLine($"Player {other.Nickname} now has a powerup!");
        }

        public abstract void Activate(Player player);
    }
}
