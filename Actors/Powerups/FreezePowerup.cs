using System.Diagnostics.CodeAnalysis;

namespace AirHockey.Actors.Powerups
{
    public class FreezePowerup : Powerup
    {
        private float _prevSpeed;
        public FreezePowerup(float x, float y, int id, float freezeDuration = 3f) : base(x, y, id)
        {
            Duration = freezeDuration;
        }

        protected sealed override void ApplyEffect(Player player)
        {
            var enemyPlayer = player.Room.Players.FirstOrDefault(p => p.Id != player.Id);
            if (enemyPlayer == null)
            {
                throw new InvalidOperationException("No valid enemy player found in the room. " + player.Room.Players.Count);
            }
            _prevSpeed = enemyPlayer.MaxSpeed;
            enemyPlayer.MaxSpeed = 0;
            enemyPlayer.VelocityX *= 0;
            enemyPlayer.VelocityY *= 0;
        }

        protected sealed override void RemoveEffect(Player player)
        {
            var enemyPlayer = player.Room.Players.FirstOrDefault(p => p.Id != player.Id);
            enemyPlayer.MaxSpeed = _prevSpeed;
        }

        public override Powerup CloneDeep()
        {
            return new FreezePowerup(this.X, this.Y, this.Id, this.Duration);
        }

        public override Powerup CloneShallow()
        {
            return (FreezePowerup)this.MemberwiseClone();
        }

        [ExcludeFromCodeCoverage]
        public override void Update()
        {

        }
    }
}
