namespace AirHockey.Handlers;

public class MovementHandler : InputHandler
{
    public override bool Handle(InputContext context)
    {
        if (context.Player == null) return false;

        float xDirection = (context.Inputs.GetValueOrDefault("left") ? -1 : 0) + 
                           (context.Inputs.GetValueOrDefault("right") ? 1 : 0);
        float yDirection = (context.Inputs.GetValueOrDefault("up") ? -1 : 0) + 
                           (context.Inputs.GetValueOrDefault("down") ? 1 : 0);

        Console.WriteLine($"Player {context.ConnectionId} accelerates in direction: ({xDirection}, {yDirection})");
        context.Player.Accelerate(xDirection, yDirection);

        PassToNext(context);
        return true;
    }
}