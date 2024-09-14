namespace AirHockey.Actors
{
    public class Room
    {
        public string RoomCode { get; private set; }
        public List<string> Players { get; private set; } // Player ConnectionIds

        public Room(string roomCode)
        {
            RoomCode = roomCode;
            Players = new List<string>();
        }
    }
}
