using AirHockey.Actors;
using AirHockey.Analytics;
using AirHockey.Handlers;
using Microsoft.AspNetCore.SignalR;

namespace AirHockey.States
{
    public class EndedState : IState
    {
        private readonly IHubContext<GameHub> _hubContext;

        public EndedState(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async void Handle(Room room, StateContext context)
        {
            // Log that the game has ended
            Console.WriteLine("Game Over. No further actions will be processed.");
            Console.WriteLine($"Final Score: Player 1 - {room.Player1Score}, Player 2 - {room.Player2Score}");

            // Determine the winner and send notifications
            if (room.Player1Score >= room.GetMaxGoal() || room.Player1Score > room.Player2Score)
            {
                Console.WriteLine("Player 1 is the winner.");
                await _hubContext.Clients.Group(room.RoomCode).SendAsync("PlayerWon", room.Players[0].Nickname, room.Player1Score);
            }
            else if (room.Player2Score >= room.GetMaxGoal() || room.Player2Score > room.Player1Score)
            {
                Console.WriteLine("Player 2 is the winner.");
                await _hubContext.Clients.Group(room.RoomCode).SendAsync("PlayerWon", room.Players[1].Nickname, room.Player2Score);
            }

            // Send GameOver notification
            await _hubContext.Clients.Group(room.RoomCode).SendAsync("GameOver", "Game over! The game has ended.");
        }
    }
}
