using AirHockey.Actors;

namespace AirHockey.Interpreters;

public abstract class GameCommand
{
    public abstract void Execute(Player player);
}