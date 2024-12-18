using AirHockey.Ambience.Effects;
using AirHockey.States;

namespace AirHockey.Handlers;

public class ResignHandler : InputHandler
{
    public override bool Handle(InputContext context)
    {
        if (context.Player == null) return false;
        if (context.Player.Room.State is PausedState)
        {
            PassToNext(context);
            return false;
        }
        
        if (context.Inputs.GetValueOrDefault("resign"))
        {
            context.Service.EndGame(context.Game, context.Player);
        }

        PassToNext(context);
        return true;
    }
}