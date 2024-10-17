using AirHockey.Actors;

namespace AirHockey.Observers
{
    public interface IGoalObserver
    {
        void OnGoalScored(Player scorer, Game game);
    }
}
