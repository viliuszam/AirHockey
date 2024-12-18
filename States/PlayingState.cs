using AirHockey.Actors;

namespace AirHockey.States
{
    public class PlayingState : IState
    {
        public void Handle(Room room, StateContext _context)
        {

            room.Players[0].VelocityX = _context.Player1VelocityX;
            room.Players[0].VelocityY = _context.Player1VelocityY;
            room.Players[0].Acceleration = _context.Player1Acceleration; 

            room.Players[1].VelocityX = _context.Player2VelocityX;
            room.Players[1].VelocityY = _context.Player2VelocityY;
            room.Players[1].Acceleration = _context.Player2Acceleration; 

            room.Puck.VelocityX = _context.PuckVelocityX;
            room.Puck.VelocityY = _context.PuckVelocityY;
            room.Puck.Acceleration = _context.PuckAcceleration;

        }
    }

}
