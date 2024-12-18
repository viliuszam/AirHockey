using AirHockey.Actors;

namespace AirHockey.States
{
    public class PausedState : IState
    {
        public void Handle(Room room, StateContext _context)
        {
            _context.Player1VelocityX = room.Players[0].VelocityX;
            _context.Player1VelocityY = room.Players[0].VelocityY;
            _context.Player1Acceleration = room.Players[0].Acceleration;

            _context.Player2VelocityX = room.Players[1].VelocityX;
            _context.Player2VelocityY = room.Players[1].VelocityY;
            _context.Player2Acceleration = room.Players[1].Acceleration;

            _context.PuckVelocityX = room.Puck.VelocityX;
            _context.PuckVelocityY = room.Puck.VelocityY;
            _context.PuckAcceleration = room.Puck.Acceleration;

            room.Players[0].VelocityX = 0;
            room.Players[0].VelocityY = 0;
            room.Players[0].Acceleration = 0;
            room.Players[1].VelocityX = 0;
            room.Players[1].VelocityY = 0;
            room.Players[1].Acceleration = 0;
            room.Puck.VelocityX = 0;
            room.Puck.VelocityY = 0;
            room.Puck.Acceleration = 0;
        }
    }
    }

