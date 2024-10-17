using AirHockey.Actors;

namespace AirHockey.Observers
{
    public class ResetPositionObserver : IGoalObserver
    {
        public void OnGoalScored(Player scorer, Game game)
        {
            game.Puck.X = 855 / 2;
            game.Puck.Y = 541 / 2;
            game.Puck.VelocityX = 0;
            game.Puck.VelocityY = 0;

            game.Room.Players[0].X = 227;
            game.Room.Players[0].Y = 260;
            game.Room.Players[1].X = 633;
            game.Room.Players[1].Y = 260;

            game.Room.Players[0].VelocityX = 0;
            game.Room.Players[0].VelocityY = 0;
            game.Room.Players[1].VelocityX = 0;
            game.Room.Players[1].VelocityY = 0;

        }
    }
}

