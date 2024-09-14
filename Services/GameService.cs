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
        // perdaryt sita
        private static ConcurrentDictionary<string, string> PlayerNicknames = new();
        private readonly IHubContext<GameHub> _hubContext;
        private System.Timers.Timer gameLoopTimer;

        // stalo dimensijos
        private const float MIN_X = 0f;
        private const float MAX_X = 855f;
        private const float MIN_Y = 0f;
        private const float MAX_Y = 541f;

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

        private void HandleCollisions(Game game)
        {
            // visi imanomi susidurimo atvejai
            if (game.Player1.IsColliding(game.Player2))
            {
                game.Player1.ResolveCollision(game.Player2);
            }
            if (game.Player1.IsColliding(game.Puck))
            {
                game.Player1.ResolveCollision(game.Puck);
            }
            if (game.Player2.IsColliding(game.Puck))
            {
                game.Player2.ResolveCollision(game.Puck);
            }
        }

        private async void GameLoop(object sender, ElapsedEventArgs e)
        {
            foreach (var game in Games.Values)
            {
                try
                {
                    game.Player1.Update();
                    game.Player2.Update();
                    game.Puck.Update();

                    HandleCollisions(game);

                    game.Player1.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    game.Player2.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    game.Puck.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);

                    await _hubContext.Clients.Group(game.Room.RoomCode).SendAsync("UpdateGameState",
                        game.Player1.X, game.Player1.Y,
                        game.Player2.X, game.Player2.Y,
                        game.Puck.X, game.Puck.Y);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in game loop: {ex.Message}");
                }
            }
        }

        public void AddPlayerNickname(string connectionId, string nickname)
        {
            PlayerNicknames[connectionId] = nickname;
        }

        public string GetPlayerNickname(string connectionId)
        {
            return PlayerNicknames.GetValueOrDefault(connectionId);
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
