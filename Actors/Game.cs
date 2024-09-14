namespace AirHockey.Actors
{
    public class Game
    {
        public Room Room { get; private set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Puck Puck { get; set; }
        public int Player1Score { get; set; } = 0;
        public int Player2Score { get; set; } = 0;

        public Game(Room room)
        {
            Room = room;
            Player1 = new Player(room.Players[0], "red", 227, 260);
            Player2 = new Player(room.Players[1], "blue", 633, 260);
            Puck = new Puck();
        }

        public void GoalScored(Player scorer)
        {
            if (scorer == Player1)
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

            Player1.X = 227;
            Player1.Y = 260;
            Player2.X = 633;
            Player2.Y = 260;

            Player1.VelocityX = 0;
            Player1.VelocityY = 0;
            Player2.VelocityX = 0;
            Player2.VelocityY = 0;
        }

        public void StartGame()
        {
        }
    }
}
