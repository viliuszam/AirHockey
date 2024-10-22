using AirHockey.Actors;

namespace AirHockey.Observers
{
    public class ResetPositionObserver : IGoalObserver
    {
        public void OnGoalScored(Player scorer, Game game)
        {
            Puck puck = game.Room.Puck;
            puck.X = 855 / 2;
            puck.Y = 541 / 2;
            puck.VelocityX = 0;
            puck.VelocityY = 0;

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

