using AirHockey.Actors;

public interface ICommand
{
    void Execute();
    void Undo();
}