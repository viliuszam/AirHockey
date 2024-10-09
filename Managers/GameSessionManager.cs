using AirHockey.Actors;
using System.Collections.Concurrent;

namespace AirHockey.Managers
{
    public class GameSessionManager
    {
        private static GameSessionManager _instance;
        private static readonly object _lock = new object();

        public ConcurrentDictionary<string, Room> ActiveRooms { get; private set; }
        public ConcurrentDictionary<string, Game> ActiveGames { get; private set; }

        private GameSessionManager()
        {
            ActiveRooms = new ConcurrentDictionary<string, Room>();
            ActiveGames = new ConcurrentDictionary<string, Game>();
        }

        public static GameSessionManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GameSessionManager();
                    }
                    return _instance;
                }
            }
        }

        public void AddRoom(Room room)
        {
            ActiveRooms.TryAdd(room.RoomCode, room);
        }

        public void RemoveRoom(string roomCode)
        {
            EndGame(roomCode);
            ActiveRooms.TryRemove(roomCode, out _);
        }

        public Room GetRoom(string roomCode)
        {
            return ActiveRooms.GetValueOrDefault(roomCode);
        }

        public bool RoomExists(string roomCode)
        {
            return ActiveRooms.ContainsKey(roomCode);
        }

        public void StartNewGame(Room room)
        {
            var game = new Game(room);
            ActiveGames.TryAdd(room.RoomCode, game);
        }

        public void EndGame(string roomCode)
        {
            ActiveGames.TryRemove(roomCode, out _);
        }

        public Game GetGame(string roomCode)
        {
            return ActiveGames.GetValueOrDefault(roomCode);
        }
    }
}
