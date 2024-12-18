using AirHockey.Actors;
using AirHockey.States;

namespace AirHockey.Handlers
{
    public class PauseHandler : InputHandler
    {
        public override bool Handle(InputContext context)
        {
            if (context.Player == null) return false;

            if (context.Inputs.GetValueOrDefault("pause"))
            {
                var currentState = context.Player.Room.GetCurrentState();

                switch (currentState)
                {
                    case PlayingState _:
                        context.Player.Room.SetState(new PausedState());
                        break;

                    case PausedState _:
                        context.Player.Room.SetState(new PlayingState());
                        break;

                    default:
                        return false;
                }
            }

            PassToNext(context);
            return true;
        }
    }
}
