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

        private Player? GetScorer(Game game)
        {
            Puck puck = game.Puck;
            if (puck.X <= GOAL_WIDTH && puck.Y >= GOAL_Y_MIN && puck.Y <= GOAL_Y_MAX)
            {
                return game.Player2;
            }

            if (puck.X >= (MAX_X - GOAL_WIDTH) && puck.Y >= GOAL_Y_MIN && puck.Y <= GOAL_Y_MAX)
            {
                return game.Player1;
            }

            return null;
        }

        private void CheckGoal(Game game)
        {
            Player scorer = GetScorer(game);
            if (scorer != null)
            {
                game.GoalScored(scorer);

                string roomCode = game.Room.RoomCode;
                var playerNicknames = GameSessionManager.Instance.PlayerNicknames;
                _hubContext.Clients.Group(roomCode).SendAsync("GoalScored", playerNicknames[scorer.Id], game.Player1Score, game.Player2Score);

                Console.WriteLine($"{scorer.Nickname} scored! Score is now {game.Player1Score} - {game.Player2Score}");
            }
        }

        private async void GameLoop(object sender, ElapsedEventArgs e)
        {
            foreach (var game in GameSessionManager.Instance.ActiveGames.Values)
            {
                try
                {
                    game.Player1.Update();
                    game.Player2.Update();
                    game.Puck.Update();

                    HandleCollisions(game);
                    CheckGoal(game);

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
    }
}
