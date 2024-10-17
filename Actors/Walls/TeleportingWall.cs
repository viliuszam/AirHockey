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

        public override void Update()
        {
            if (lastTeleportedEntity != null)
            {
                lastTeleportedEntity = null;
            }
        }
        public Entity? GetLast()
        {
            return lastTeleportedEntity;
        }
        public void SetLast(Entity entity)
        {
            lastTeleportedEntity = entity;
        }
    }
}