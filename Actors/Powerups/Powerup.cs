namespace AirHockey.Actors.Powerups
{
    public abstract class Powerup
    {
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsActive { get; private set; }

        public Powerup(float x, float y)
        {
            X = x;
            Y = y;
            IsActive = true;
        }

        public virtual void OnPickup(Player player)
        {
            IsActive = false;
        }

        public abstract void Activate(Player player);
    }
}
