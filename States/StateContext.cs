using AirHockey.Actors;

namespace AirHockey.States
{
    public class StateContext(Room room)
    {
        public float Player1VelocityX { get; set; } = room.Players[0].VelocityX;
        public float Player1VelocityY { get; set; } = room.Players[0].VelocityY;
        public float Player1Acceleration { get; set; } = room.Players[0].Acceleration;

        public float Player2VelocityX { get; set; } = room.Players[1].VelocityX;
        public float Player2VelocityY { get; set; } = room.Players[1].VelocityY;
        public float Player2Acceleration { get; set; } = room.Players[1].Acceleration;

        public float PuckVelocityX { get; set; } = room.Puck.VelocityX;
        public float PuckVelocityY { get; set; } = room.Puck.VelocityY;
        public float PuckAcceleration { get; set; } = room.Puck.Acceleration;
    }
}
