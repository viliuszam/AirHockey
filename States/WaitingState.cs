using AirHockey.Actors;

namespace AirHockey.States
{
    public class WaitingState : IState
    {
        public void Handle(Room room, StateContext _context)
        {
            Console.WriteLine("Waiting for other player");
        }
    }
}
