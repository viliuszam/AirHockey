namespace AirHockey.Actors
{
    public class Room
    {
        public string RoomCode { get; private set; }
        public List<string> Players { get; private set; }

        public Room(string roomCode)
        {
            RoomCode = roomCode;
            Players = new List<string>();
        }
    }
}
