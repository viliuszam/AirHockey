using AirHockey.Actors;

namespace AirHockey.States
{
    public class PausedState : IState
    {
        public void Handle(Room room)
        {
            room.Players[0].VelocityX = 0;
            room.Players[0].VelocityY = 0;
            room.Players[1].VelocityX = 0;
            room.Players[1].VelocityY = 0;
        }
    }
}
