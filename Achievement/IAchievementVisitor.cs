using AirHockey.Actors;

namespace AirHockey.Achievement
{
    public interface IAchievementVisitor
    {
        void Visit(Player player) {}

        void Visit(Room room) {}
    }
}