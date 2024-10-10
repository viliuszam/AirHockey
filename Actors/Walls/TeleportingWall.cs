using System;

namespace AirHockey.Actors.Walls
{
    public class TeleportingWall : Wall
    {
        public TeleportingWall? LinkedWall { get; private set; } = null;
        public bool IsLinked { get; private set; } = false;
        private Entity? lastTeleportedEntity = null;
        public TeleportingWall(int id, float width, float height) 
            : base(id, width, height)
        {
        }

        public bool LinkWall(TeleportingWall otherWall)
        {
            if (!IsLinked && otherWall != this)
            {
                LinkedWall = otherWall;
                otherWall.LinkedWall = this;
                IsLinked = true;
                return true;
            }
            return false;
        }

        public override void ResolveCollision(Entity other)
        {
            if (!IsLinked || LinkedWall == null) return;
            if (lastTeleportedEntity == other) return;
            if(other is Wall){
                return;
            }
            other.X = LinkedWall.X + LinkedWall.Width / 2;
            other.Y = LinkedWall.Y + LinkedWall.Height / 2;

            LinkedWall.lastTeleportedEntity = other;
            lastTeleportedEntity = other;

        }

        public override void Update()
        {
            if (lastTeleportedEntity != null)
            {
                lastTeleportedEntity = null;
            }
        }
    }
}