using AirHockey.Actors;

namespace AirHockey.Handlers;

public class PauseHandler : InputHandler
{
    public override bool Handle(InputContext context)
    {
        if (context.Player == null) return false;
        if (context.Inputs.GetValueOrDefault("pause"))
        {
            switch (context.Player.Room.State)
            {
                case Room.RoomState.Playing:
                    context.Player.Room.SetState(Room.RoomState.Paused);
                    context.Player.Room.Players[0].VelocityX = 0;
                    context.Player.Room.Players[0].VelocityY = 0;
                    context.Player.Room.Players[1].VelocityX = 0;
                    context.Player.Room.Players[1].VelocityY = 0;
                    break;
                case Room.RoomState.Paused:
                    context.Player.Room.SetState(Room.RoomState.Playing);
                    break;
                default:
                    return false;
            }
        }
        PassToNext(context);
        return true;
    }
}