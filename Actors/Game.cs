using AirHockey.Effects;
using AirHockey.Observers;

namespace AirHockey.Actors
{
    public class Game: IObserver
    {
        public Room Room { get; private set; }
        public int Player1Score { get; set; } = 0;
        public int Player2Score { get; set; } = 0;
        public bool IsInitialized { get; set; } = false;

        public List<EnvironmentalEffect> ActiveEffects;
        int lastScorer;

        public Game(Room room)
        {
            Room = room;
            ActiveEffects = new List<EnvironmentalEffect>();
            lastScorer = -1;
        }

        public void GoalScored(int scorer)
        {
            if (scorer == 0) //Player1
                Player1Score++;
            else
                Player2Score++;
            lastScorer = scorer;
            ResetPositions();
        }
        public int GetLast()
        {
            return lastScorer;
        }
        public void SetLast()
        {
            lastScorer = -1;
        }
        private void ResetPositions()
        {
            Room.Puck.X = 855 / 2;
            Room.Puck.Y = 541 / 2;
            Room.Puck.VelocityX = 0;
            Room.Puck.VelocityY = 0;

            Room.Players[0].X = 227;
            Room.Players[0].Y = 260;
            Room.Players[1].X = 633;
            Room.Players[1].Y = 260;

            Room.Players[0].VelocityX = 0;
            Room.Players[0].VelocityY = 0;
            Room.Players[1].VelocityX = 0;
            Room.Players[1].VelocityY = 0;
        }
        public void StartGame()
        {
            Player1Score = 0;
            Player2Score = 0;
        }
    }
}
