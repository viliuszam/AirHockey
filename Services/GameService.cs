using AirHockey.Actors;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Timers;

namespace AirHockey.Services
{
    public class GameService
    {
        private static ConcurrentDictionary<string, Room> Rooms = new();
        private static ConcurrentDictionary<string, Game> Games = new();
        private readonly IHubContext<GameHub> _hubContext;
        private System.Timers.Timer gameLoopTimer;

        public GameService(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;

            gameLoopTimer = new System.Timers.Timer(16);  // 16*60 ~ apie 60 fps
            gameLoopTimer.Elapsed += GameLoop;
            gameLoopTimer.Start();
        }

        public void CreateGame(Room room)
        {
            var game = new Game(room);
            Games[room.RoomCode] = game;
        }

        private async void GameLoop(object sender, ElapsedEventArgs e)
        {
            foreach (var game in Games.Values)
            {
                game.Player1.Update();
                game.Player2.Update();
                game.Puck.Update();

                Console.WriteLine($"Sending state for game {game.Room.RoomCode}: Player1({game.Player1.X}, {game.Player1.Y}), " +
                  $"Player2({game.Player2.X}, {game.Player2.Y}), Puck({game.Puck.X}, {game.Puck.Y})");

                await _hubContext.Clients.Group(game.Room.RoomCode).SendAsync("UpdateGameState",
                    game.Player1.X, game.Player1.Y,
                    game.Player2.X, game.Player2.Y,
                    game.Puck.X, game.Puck.Y);
            }
        }

        public ConcurrentDictionary<string, Room> GetRooms()
        {
            return Rooms;
        }

        public Room GetRoom(string roomCode) => Rooms.GetValueOrDefault(roomCode);
        public Game GetGame(string roomCode) => Games.GetValueOrDefault(roomCode);

        public bool RoomExists(string roomCode) => Rooms.ContainsKey(roomCode);

        public void AddRoom(Room room) => Rooms[room.RoomCode] = room;

        public void RemoveRoom(string roomCode)
        {
            Rooms.TryRemove(roomCode, out _);
            Games.TryRemove(roomCode, out _);
        }
    }
}
