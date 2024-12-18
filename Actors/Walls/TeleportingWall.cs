using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls
{
    public class TeleportingWall : Wall
    {
        public TeleportingWall? LinkedWall { get; private set; } = null;
        public bool IsLinked { get; private set; } = false;
        private Entity? lastTeleportedEntity = null;

        // Updated constructor: Accept FlyweightWall and pass it to the base Wall constructor
        public TeleportingWall(int id, FlyweightWall flyweight)
            : base(id, flyweight)  // Pass the flyweight to the Wall constructor
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
