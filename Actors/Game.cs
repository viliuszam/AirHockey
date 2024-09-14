namespace AirHockey.Actors
{
    public class Game
    {
        public Room Room { get; private set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Puck Puck { get; set; }

        public Game(Room room)
        {
            Room = room;
            Player1 = new Player(room.Players[0], "red", 227, 260);
            Player2 = new Player(room.Players[1], "blue", 633, 260);
            Puck = new Puck();
        }

        public void StartGame()
        {
        }
    }

}
