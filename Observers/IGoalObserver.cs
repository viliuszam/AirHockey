using AirHockey.Actors;

// Room stebi Powerup, Sienu busena

namespace AirHockey.Observers
{
    public interface IGoalObserver
    {
        void OnGoalScored(Player scorer, Game game);
    }
}
