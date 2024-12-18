using AirHockey.Actors;

namespace AirHockey.Interpreters;

public interface IGameCommand
{
    void Execute(Player player);
}