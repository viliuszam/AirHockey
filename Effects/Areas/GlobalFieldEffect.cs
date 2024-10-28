using AirHockey.Actors;

namespace AirHockey.Effects.Areas
{

    public class GlobalFieldEffect : EnvironmentalEffect
    {

        private bool isApplied;

        private bool Reapply;

        public GlobalFieldEffect(int ID, IEffectBehavior behavior, float duration, bool reapply) : base(ID, behavior, duration) {
            isApplied = false;
            Reapply = reapply;
        }

        public override void ApplyEffect(Room room)
        {
            if (!isApplied || Reapply)
            {
                foreach (var entity in room.Players.Concat<Entity>(new[] { room.Puck }))
                {
                    _behavior.Execute(entity);
                }

                isApplied = true;
            }

        }

        public override void RemoveEffect(Room room)
        {
            if (isApplied)
            {
                foreach (var entity in room.Players.Concat<Entity>(new[] { room.Puck }))
                {
                    _behavior.Revert(entity);
                }
                isApplied = false;
            }
        }
    }
}
