using AirHockey.Actors;
using AirHockey.States;
using Microsoft.AspNetCore.SignalR;

public class EndedState : IState
{
    private readonly IHubContext<GameHub> _hubContext;
    private readonly string _winnerNickname;
    private readonly int _winnerScore;

    public EndedState(IHubContext<GameHub> hubContext, string winnerNickname, int winnerScore)
    {
        _hubContext = hubContext;
        _winnerNickname = winnerNickname;
        _winnerScore = winnerScore;
    }

    public async void Handle(Room room, StateContext context)
    {
        // Log that the game has ended
        Console.WriteLine("Game Over. No further actions will be processed.");
        Console.WriteLine($"Final Score: Player 1 - {room.Player1Score}, Player 2 - {room.Player2Score}");

        // Notify the winner
        Console.WriteLine($"{_winnerNickname} is the winner with a score of {_winnerScore}.");
        await _hubContext.Clients.Group(room.RoomCode).SendAsync("PlayerWon", _winnerNickname, _winnerScore);

        // Send GameOver notification
        await _hubContext.Clients.Group(room.RoomCode).SendAsync("GameOver", "Game over! The game has ended.");
    }
}
