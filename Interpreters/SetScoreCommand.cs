using AirHockey.Actors;

namespace AirHockey.Interpreters;

public class SetScoreCommand : IGameCommand
{
    private int _score;

    public SetScoreCommand(int score)
    {
        _score = score;
    }

    public void Execute(Player player)
    {
        if (player == null) return;
        var playerIndex = player.Room.Players.FindIndex(a => a == player);
        if (playerIndex == 0)
        {
            player.Room.Player1Score = _score;
        }
        else
        {
            player.Room.Player2Score = _score;
        }
        Console.WriteLine($"Player's score set to {_score}");
    }
}