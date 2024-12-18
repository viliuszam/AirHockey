using AirHockey.Actors;

namespace AirHockey.States
{
    public interface IState
    {
        void Handle(Room room, StateContext _context);
    }
}
