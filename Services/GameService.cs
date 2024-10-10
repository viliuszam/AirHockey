﻿using AirHockey.Actors;
using AirHockey.Actors.Walls;
using AirHockey.Managers;
using Microsoft.AspNetCore.SignalR;
using System.Drawing;
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

            foreach (var wall in game.Room.Walls)
            {
                if (wall.IsColliding(player1))
                {
                    wall.ResolveCollision(player1);
                }
                if (wall.IsColliding(player2))
                {
                    wall.ResolveCollision(player2);
                }
                if (wall.IsColliding(puck))
                {
                    wall.ResolveCollision(puck);
                }

                foreach (var otherWall in game.Room.Walls)
                {
                    if (wall != otherWall && wall.IsColliding(otherWall))
                    {
                        wall.ResolveCollision(otherWall);

                    }
                }
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
                    
                    foreach (var wall in game.Room.Walls)
                    {
                        wall.Update();
                        wall.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    }

                    HandleCollisions(game);
                    CheckGoal(game);

                    player1.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    player2.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    puck.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);


                    var sentWalls = game.Room.Walls
                        .Select(wall => new
                        {
                            wall.Id,
                            wall.X,
                            wall.Y,
                            wall.Width,
                            wall.Height,
                            wall.GetType().Name
                        })
                        .ToList();


                    await _hubContext.Clients.Group(game.Room.RoomCode).SendAsync("UpdateGameState",
                        player1.X, player1.Y,
                        player2.X, player2.Y,
                        game.Puck.X, game.Puck.Y, sentWalls);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in game loop: {ex.Message}");
                }
            }
        }
        private static Random random = new Random();
        public void GenerateWalls(Room room)
        {
            int numberOfWalls = random.Next(1, 10);

            AbstractWallFactory StaticWallFactory = new StaticWallFactory();
            AbstractWallFactory DynamicWallFactory = new DynamicWallFactory();

            List<Wall> wallsToAdd = new List<Wall>();

            int teleporterCount = 0;

            for (int i = 0; i < numberOfWalls;)
            {
                Wall? wall = null;
                bool isValidPosition = false;


                while (!isValidPosition)
                {
                    float x = 0, y = 0, width = 0, height = 0;
                    int wallType = random.Next(1, 7);

                    isValidPosition = true;
                    x = (float)(35 + random.NextDouble() * (705 - 35));
                    y = (float)(20 + random.NextDouble() * (391 - 20));
                    width = (float)(10 + random.NextDouble() * (150 - 10));
                    height = width > 75 ? (float)(10 + random.NextDouble() * (75 - 10)) : (float)(75 + random.NextDouble() * (150 - 75));
                    
                    switch (wallType)
                    {
                        
                        case 1:
                            if (teleporterCount != 0)  // Only allow 2 teleporters
                            {
                                continue;
                            }

                            // Teleporter 1
                            wall = StaticWallFactory.CreateWall(i, width, height, "Teleporting");
                            wall.X = x;
                            wall.Y = y;

                            // Teleporter 2
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                i++;
                                bool found = false;
                                while (!found)
                                {
                                    Wall? wall2 = null;
                                    float x2 = 0, y2 = 0, width2 = 0, height2 = 0;
                                    x2 = (float)(50 + random.NextDouble() * (705 - 50));
                                    y2 = (float)(50 + random.NextDouble() * (391 - 50));
                                    width2 = (float)(10 + random.NextDouble() * (150 - 10));
                                    height2 = width2 > 75 ? (float)(10 + random.NextDouble() * (75 - 10)) : (float)(75 + random.NextDouble() * (150 - 75));       
                                    wall2 = StaticWallFactory.CreateWall(i, width2, height2, "Teleporting");
                                    wall2.X = x2;
                                    wall2.Y = y2;

                                    if (IsPositionValid(wall2, room, wallsToAdd))
                                    {
                                        teleporterCount += 1; // Increment teleporter count

                                        if (wall is TeleportingWall teleporterWall1 && wall2 is TeleportingWall teleporterWall2)
                                        {
                                            teleporterWall1.LinkWall(teleporterWall2);

                                            wallsToAdd.Add(teleporterWall1);
                                            wallsToAdd.Add(teleporterWall2);
                                        }
                                        isValidPosition = true;
                                        found = true;
                                        i++;
                                    }
                                }
                            }
                            break;
                        case 2:
                            wall = StaticWallFactory.CreateWall(i, width, height, "QuickSand");
                            isValidPosition = true;
                            wall.X = x;
                            wall.Y = y;
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                isValidPosition = true;
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 3:
                            wall = StaticWallFactory.CreateWall(i, width, height, "Standard");
                            wall.X = x;
                            wall.Y = y;
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                isValidPosition = true;
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 4:
                            wall = StaticWallFactory.CreateWall(i, width, height, "Bouncy");
                            wall.X = x;
                            wall.Y = y;
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                isValidPosition = true;
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 5:
                            wall = DynamicWallFactory.CreateWall(i, width, height, "Bouncy");
                            wall.X = x;
                            wall.Y = y;
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                isValidPosition = true;
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 6:
                            wall = DynamicWallFactory.CreateWall(i, width, height, "Scrolling");
                            wall.X = x;
                            wall.Y = y;
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                isValidPosition = true;
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (var newWall in wallsToAdd)
            {
                room.Walls.Add(newWall);
            }
        }

        private bool IsPositionValid(Wall wall, Room room, List<Wall> wallsToAdd)
        {
            // Define exclusion zones around players and puck (25x25 space around them)
            var exclusionZones = new List<Rectangle>
            {
                new Rectangle(175, 220, 100, 100),
                new Rectangle(575, 220, 100, 100),
                new Rectangle(375, 220, 100, 100)
            };

            // Check if the wall collides with any exclusion zone
            foreach (var exclusionZone in exclusionZones)
            {
                if (wall.IsColliding(exclusionZone))
                {
                    return false;
                }
            }

            // Check if the wall is colliding with any walls that are not yet added (wallsToAdd)
            foreach (var newWall in wallsToAdd)
            {
                if (wall.IsColliding(newWall))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
