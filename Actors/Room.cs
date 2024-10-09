namespace AirHockey.Actors
{
    public class Room
    {
        public string RoomCode { get; private set; }
        public List<Player> Players { get; private set; }

        public Room(string roomCode)
        {
            RoomCode = roomCode;
            Players = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            if (Players.Count < 2)
            {
                Players.Add(player);
            }
            else
            {
                throw new InvalidOperationException("Room is already full.");
            }
        }

        public Player GetPlayerById(string connectionId)
        {
            return Players.FirstOrDefault(p => p.Id == connectionId);
        }

        public void RemovePlayer(string connectionId)
        {
            var player = GetPlayerById(connectionId);
            if (player != null)
            {
                Players.Remove(player);
            }
        }

        public bool IsRoomFull()
        {
            return Players.Count >= 2;
        }
    }
}
