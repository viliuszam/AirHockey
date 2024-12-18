using AirHockey.Actors;
using AirHockey.Analytics;
using AirHockey.Handlers;
namespace AirHockey.States
{
    public class EndedState : IState
    {
        public void Handle(Room room, StateContext context)
        {
            // Log that the game has ended
            Console.WriteLine("Game Over. No further actions will be processed.");

            Console.WriteLine($"Final Score: Player 1 - {room.Player1Score}, Player 2 - {room.Player2Score}");

        }
    }
}
