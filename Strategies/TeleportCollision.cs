using AirHockey.Actors;
using AirHockey.Actors.Walls;
using System.Runtime.CompilerServices;

namespace AirHockey.Strategies
{
    public class TeleportCollision : ICollision
    {
        public void ResolveCollision(Entity teleport, Entity other)
        {
            if (teleport is TeleportingWall teleportingWall && teleportingWall.IsLinked == false && other != teleport) return;
            if (teleport is TeleportingWall teleportingWall1 && teleportingWall1.GetLast() == other) return;
            if (other is Wall)
            {
                return;
            }
            if (teleport is TeleportingWall tele && tele.LinkedWall != null)
            {
                other.X = tele.LinkedWall.X + tele.LinkedWall.Width / 2;
                other.Y = tele.LinkedWall.Y + tele.LinkedWall.Height / 2;

                tele. LinkedWall.SetLast(other);
                tele.SetLast(other);
            }
        }
    }
}
