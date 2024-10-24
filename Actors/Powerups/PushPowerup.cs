namespace AirHockey.Actors.Powerups
{
    public class PushPowerup : Powerup
    {
        private readonly float _pushForce;

        public PushPowerup(float x, float y, int id, float pushForce = 5f) : base(x, y, id)
        {
            _pushForce = pushForce;
        }

        public override void Activate(Player player)
        {
            // Apply force to all other players in the room
            foreach (var otherPlayer in player.Room.Players)
            {
                if (otherPlayer != player)
                {
                    ApplyPushForce(otherPlayer, player.AngleFacing, _pushForce);
                }
            }

            // Apply force to the puck
            ApplyPushForce(player.Room.Puck, player.AngleFacing, _pushForce);
        }

        private void ApplyPushForce(Entity entity, float angle, float force)
        {
            float pushX = (float)Math.Cos(angle) * force;
            float pushY = (float)Math.Sin(angle) * force;

            entity.VelocityX += pushX;
            entity.VelocityY += pushY;
        }

        public override void Update()
        {
            
        }
    }
}
