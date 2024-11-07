using AirHockey.Actors.Walls;
using AirHockey.Actors.Powerups;
using AirHockey.Observers;

namespace AirHockey.Actors
{
    public class Room : IObserver
    {
        public string RoomCode { get; private set; }
        public List<Player> Players { get; set; }
        public List<Wall> Walls { get; set; } = new List<Wall>();
        public List<Powerup> Powerups { get; set; } = new List<Powerup>();
        public Puck Puck { get; set; }
        public int Player1Score { get; set; } = 0;
        public int Player2Score { get; set; } = 0;
        int lastScorer;
        public Room(string roomCode)
        {
            RoomCode = roomCode;
            Players = new List<Player>();
            Puck = new Puck();
            lastScorer = -1;
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
            Puck.X = 855 / 2;
            Puck.Y = 541 / 2;
            Puck.VelocityX = 0;
            Puck.VelocityY = 0;

            Players[0].X = 227;
            Players[0].Y = 260;
            Players[1].X = 633;
            Players[1].Y = 260;

            Players[0].VelocityX = 0;
            Players[0].VelocityY = 0;
            Players[1].VelocityX = 0;
            Players[1].VelocityY = 0;
        }
    }
}
