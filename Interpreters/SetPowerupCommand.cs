using AirHockey.Actors;
using AirHockey.Actors.Powerups;
using AirHockey.States;
namespace AirHockey.Interpreters;

public class SetPowerupCommand : IGameCommand
{
    private string _powerup;

    public SetPowerupCommand(string powerup)
    {
        _powerup = powerup;
    }
    
    public void Execute(Player player)
    {
        if (player == null) return;
        if (player.Room.State is PausedState) return;
        var powerupFactory = new PowerupFactory();
        var powerup = powerupFactory.CreatePowerup(0, 0, 0, _powerup);
        player.ActivePowerup = powerup;
        Console.WriteLine($"Player's powerup set to {powerup}");
    }
}