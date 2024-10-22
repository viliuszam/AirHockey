using AirHockey.Actors;

namespace AirHockey.Effects.Areas
{
    public class LocalFieldEffect : EnvironmentalEffect
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }

        private HashSet<Entity> affectedEntities = new HashSet<Entity>();

        // should effect be reapplied every game tick
        private bool Reapply;

        public LocalFieldEffect(int id, IEffectBehavior behavior, float duration, float x, float y, float radius, bool reapply)
            : base(id, behavior, duration)
        {
            X = x;
            Y = y;
            Radius = radius;
            Reapply = reapply;
        }

        public override void ApplyEffect(Room room)
        {
            foreach (var entity in room.Players.Concat<Entity>(new[] { room.Puck }))
            {
                bool inRange = IsEntityInRange(entity);

                if (inRange && (!affectedEntities.Contains(entity) || Reapply))
                {
                    _behavior.Execute(entity);
                    if(!affectedEntities.Contains(entity)) affectedEntities.Add(entity); 
                }
                else if (!inRange && affectedEntities.Contains(entity))
                {
                    _behavior.Revert(entity);
                    affectedEntities.Remove(entity);
                }
            }
        }

        public override void RemoveEffect(Room room)
        {
            foreach (var entity in affectedEntities.ToList())
            {
                _behavior.Revert(entity);
                affectedEntities.Remove(entity);
            }
        }

        private bool IsEntityInRange(Entity entity)
        {
            float dx = entity.X - X;
            float dy = entity.Y - Y;
            return (dx * dx + dy * dy) <= (Radius * Radius);
        }
    }
}
