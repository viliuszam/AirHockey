namespace AirHockey.Handlers;

public abstract class InputHandler
{
    protected InputHandler _nextHandler;

    public InputHandler SetNext(InputHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public abstract bool Handle(InputContext context);

    protected void PassToNext(InputContext context)
    {
        _nextHandler?.Handle(context);
    }
}
