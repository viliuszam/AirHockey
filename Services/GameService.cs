using AirHockey.Actors;
using AirHockey.Managers;
using Microsoft.AspNetCore.SignalR;
using System.Timers;

namespace AirHockey.Services
{
    public class GameService
    {
        private readonly IHubContext<GameHub> _hubContext;
        private System.Timers.Timer gameLoopTimer;

        private const float MIN_X = 0f;
        private const float MAX_X = 855f;
        private const float MIN_Y = 0f;
        private const float MAX_Y = 541f;

        private const float GOAL_WIDTH = 25f;
        private const float GOAL_Y_MIN = 180f;
        private const float GOAL_Y_MAX = 365f;

        public GameService(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;

            gameLoopTimer = new System.Timers.Timer(16);  // 16*60 ~ apie 60 FPS
            gameLoopTimer.Elapsed += GameLoop;
            gameLoopTimer.Start();
        }

        private void HandleCollisions(Game game)
        {
            var player1 = game.Room.Players[0];
            var player2 = game.Room.Players[1];
            var puck = game.Puck;

            if (player1.IsColliding(player2))
            {
                player1.ResolveCollision(player2);
            }
            if (player1.IsColliding(puck))
            {
                player1.ResolveCollision(puck);
            }
            if (player2.IsColliding(puck))
            {
                player2.ResolveCollision(puck);
            }
        }

        private Player? GetScorer(Game game)
        {
            var player1 = game.Room.Players[0];
            var player2 = game.Room.Players[1];
            var puck = game.Puck;
            if (puck.X <= GOAL_WIDTH && puck.Y >= GOAL_Y_MIN && puck.Y <= GOAL_Y_MAX)
            {
                return player2;
            }

            if (puck.X >= (MAX_X - GOAL_WIDTH) && puck.Y >= GOAL_Y_MIN && puck.Y <= GOAL_Y_MAX)
            {
                return player1;
            }

            return null;
        }

        private async void CheckGoal(Game game)
        {
            Player? scorer = GetScorer(game);
            if (scorer != null)
            {
                game.GoalScored(scorer);

                string roomCode = game.Room.RoomCode;

                await _hubContext.Clients.Group(roomCode).SendAsync("GoalScored",
                    scorer.Nickname, game.Player1Score, game.Player2Score);

                Console.WriteLine($"{scorer.Nickname} scored! Score is now {game.Player1Score} - {game.Player2Score}");
            }
        }

        private async void GameLoop(object sender, ElapsedEventArgs e)
        {
            foreach (var game in GameSessionManager.Instance.ActiveGames.Values)
            {
                try
                {
                    var player1 = game.Room.Players[0];
                    var player2 = game.Room.Players[1];
                    var puck = game.Puck;

                    player1.Update();
                    player2.Update();
                    puck.Update();

                    HandleCollisions(game);
                    CheckGoal(game);

                    player1.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    player2.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    puck.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);

                    await _hubContext.Clients.Group(game.Room.RoomCode).SendAsync("UpdateGameState",
                        player1.X, player1.Y,
                        player2.X, player2.Y,
                        game.Puck.X, game.Puck.Y);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in game loop: {ex.Message}");
                }
            }
        }
    }
}
