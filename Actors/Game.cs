namespace AirHockey.Actors
{
    public class Game
    {
        public Room Room { get; private set; }
        public Puck Puck { get; set; }
        public int Player1Score { get; set; } = 0;
        public int Player2Score { get; set; } = 0;

        public Game(Room room)
        {
            Room = room;
            Puck = new Puck();
        }

        public void GoalScored(Player scorer)
        {
            if (scorer == Room.Players[0]) //Player1
                Player1Score++;
            else
                Player2Score++;

            ResetPositions();
        }

        private void ResetPositions()
        {
            Puck.X = 855 / 2;
            Puck.Y = 541 / 2;
            Puck.VelocityX = 0;
            Puck.VelocityY = 0;

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
