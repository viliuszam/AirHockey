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

                // Using a switch to handle state transitions
                switch (currentState)
                {
                    case PlayingState _:
                        // Transition to the Paused state
                        context.Player.Room.SetState(new PausedState());
                        break;

                    case PausedState _:
                        // If it's already paused, transition back to Playing
                        context.Player.Room.SetState(new PlayingState());
                        break;

                    default:
                        return false;
                }
            }

            // Pass the context to the next handler (if any)
            PassToNext(context);
            return true;
        }
    }
}
