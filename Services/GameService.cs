using AirHockey.Actors;
using AirHockey.Actors.Walls;
using AirHockey.Actors.Powerups;
using AirHockey.Analytics;
using AirHockey.Managers;
using AirHockey.Strategies;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Drawing;
using System.Timers;
using System.Xml.Linq;
using AirHockey.Effects;
using System.Threading;
using AirHockey.Effects.Areas;
using AirHockey.Effects.Behaviors;
using AirHockey.Actors.Powerups.PowerupDecorators;
using AirHockey.Actors.Command;
using System.Diagnostics.CodeAnalysis;
using AirHockey.Facades;
using AirHockey.Ambience.Effects;

namespace AirHockey.Services
{
    [ExcludeFromCodeCoverage]
    public class GameService
    {
        private readonly IGameAnalytics _analytics;
        private readonly IHubContext<GameHub> _hubContext;
        private System.Timers.Timer gameLoopTimer;
        private const float MIN_X = 0f; 
        private const float MAX_X = 855f;
        private const float MIN_Y = 0f;
        private const float MAX_Y = 541f;

        private const int maxEffects = 5;

        private Facade facade;

        public GameService(IHubContext<GameHub> hubContext, IGameAnalytics analytics)
        {
            _hubContext = hubContext;
            _analytics = analytics;
            gameLoopTimer = new System.Timers.Timer(16);  // 16*60 ~ apie 60 FPS
            gameLoopTimer.Elapsed += GameLoop;
            gameLoopTimer.Start();
            facade = new Facade();
        }
        public void SetStrategy(ICollision newCollisionStrategy)
        {
            facade.SetStrategy(newCollisionStrategy);
        }
        public List<Powerup> GetPowerupsByRoom(Room room)
        {
            return facade.GetAllPowerupsByRoom(room);
        }
        private void InitializeCommands(Game game)
        {
            facade.InitializeCommands(game);
        }

        public bool RoomExists(string room)
        {
            return GameSessionManager.Instance.RoomExists(room);
        }

        private void TrackEntityMovement()
        {
            facade.TrackEntityMovement();
        }

        public void UpdateEnvironmentalEffects(Game game)
        {
            if (ShouldAddNewEffect(game) 
                && GetRandomEffect(game.ActiveEffects) is var newEffect && newEffect != null)
            {
                Console.WriteLine($"Adding effect with ID: {newEffect.ID}");
                game.ActiveEffects.Add(newEffect);
            }

            for (int i = game.ActiveEffects.Count - 1; i >= 0; i--)
            {
                var effect = game.ActiveEffects[i];

                effect.ApplyEffect(game.Room);

                /*
                if (effect is LocalFieldEffect localFieldEffect)
                {
                    localFieldEffect.ApplyEffect(game.Room);
                }

                if (effect is GlobalFieldEffect globalFieldEffect)
                {
                    globalFieldEffect.ApplyEffect(game.Room);
                }*/

                effect.Duration--;
                if (effect.Duration <= 0)
                {
                    effect.RemoveEffect(game.Room);
                    game.ActiveEffects.RemoveAt(i);
                }
            }
        }

        private bool ShouldAddNewEffect(Game game)
        {
            if (game.ActiveEffects.Count >= maxEffects)
            {
                return false;
            }

            return new Random().NextDouble() < 0.005;
        }

        private EnvironmentalEffect? GetRandomEffect(List<EnvironmentalEffect> exclusions)
        {
            float GetRandomX() => (float)(random.NextDouble() * (MAX_X - MIN_X) + MIN_X);
            float GetRandomY() => (float)(random.NextDouble() * (MAX_Y - MIN_Y) + MIN_Y);

            var effects = new List<EnvironmentalEffect>
            {
                //new GlobalFieldEffect(1, new LowGravityBehavior(), 60 * 5, false),
                //new GlobalFieldEffect(2, new WindBehavior(), 60 * 5, true),
                new LocalFieldEffect(3, new LowGravityBehavior(), 60 * 3, GetRandomX(), GetRandomY(), 100f, false),
                new LocalFieldEffect(4, new WindBehavior(), 60 * 3, GetRandomX(), GetRandomY(), 70f, true),
                new LocalFieldEffect(5, new LowGravityBehavior(), 60 * 3, GetRandomX(), GetRandomY(), 100f, false),
                new LocalFieldEffect(6, new WindBehavior(), 60 * 3, GetRandomX(), GetRandomY(), 80f, true),
                new LocalFieldEffect(7, new LowGravityBehavior(), 60 * 3, GetRandomX(), GetRandomY(), 100f, false),
                new LocalFieldEffect(8, new WindBehavior(), 60 * 3, GetRandomX(), GetRandomY(), 65f, true),
                new LocalFieldEffect(9, new LowGravityBehavior(), 60 * 3, GetRandomX(), GetRandomY(), 120f, false),
                new LocalFieldEffect(10, new WindBehavior(), 60 * 3, GetRandomX(), GetRandomY(), 75f, true)
            };

            var availableEffects = effects.Where(effect =>
                !exclusions.Any(exclusion => exclusion.ID == effect.ID)
            ).ToList();

            if (availableEffects.Count == 0)
                return null;

            return effects[new Random().Next(availableEffects.Count)];
        }

        private async void CheckGoal(Game game)
        {
            if (game.Room.GetLast() != -1 && game.Room.Players != null && game.Room.Players.Count > 0)
            {
                int lastIndex = game.Room.GetLast();

                if (lastIndex >= 0 && lastIndex < game.Room.Players.Count)
                {
                    string roomCode = game.Room.RoomCode;

                    await _hubContext.Clients.Group(roomCode).SendAsync("GoalScored",
                        game.Room.Players[lastIndex].Nickname, game.Room.Player1Score, game.Room.Player2Score);

                    // Log goal
                    _analytics.LogEvent(roomCode, "GoalScored", new Dictionary<string, object>
            {
                { "ScoringPlayer", game.Room.Players[lastIndex].Nickname },
                { "Score", $"{game.Room.Player1Score} - {game.Room.Player2Score}" },
                { "TimeStamp", DateTime.Now }
            });

                    // Add a lighting effect on goal area after goal scored
                    Rectangle goalArea = lastIndex == 0 ? new Rectangle(830, 180, 25, 185) : new Rectangle(0, 180, 25, 185);
                    game.LightingEffects.AddEffect(new LightingEffect(goalArea, 1500, Color.FromArgb(100, 255, 255, 0)));

                    // play goal sound
                    game.SoundEffects.AddEffect(new SoundEffect(SoundType.GoalScored, 0.2f));

                    Console.WriteLine($"{game.Room.Players[lastIndex].Nickname} scored! Score is now {game.Room.Player1Score} - {game.Room.Player2Score}");
                    game.Room.SetLast();
                }
                else
                {
                    Console.WriteLine($"Invalid last index: {lastIndex}. Player list count: {game.Room.Players.Count}");
                }
            }
            else
            {
                Console.WriteLine("No recent goal or invalid player collection.");
            }
        }


        private async void GameLoop(object sender, ElapsedEventArgs e)
        {
            foreach (var game in GameSessionManager.Instance.ActiveGames.Values)
            {
                try
                {
                    if (!game.IsInitialized)
                    {
                        InitializeCommands(game);
                        game.IsInitialized = true;
                        game.Room.Puck.Attach(game.Room);
                    }
                    var player1 = game.Room.Players[0];
                    var player2 = game.Room.Players[1];
                    var puck = game.Room.Puck;
                    player1.Update();
                    player2.Update();
                    puck.Update();
                    
                    TrackEntityMovement();

                    foreach (var wall in game.Room.Walls)
                    {
                        wall.Update();
                        wall.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    }

                    facade.HandleCollisions(game);
                    CheckGoal(game);

                    player1.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    player2.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    puck.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);


                    var activePowerups = facade.GetActivePowerups(game);

                    var playerActivePowerups = facade.GetPlayerActivePowerups(player1,player2);

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

                    UpdateEnvironmentalEffects(game);

                    var activeEffects = game.ActiveEffects
                        .Select(eff => new
                        {
                            EffectType = eff.GetType().Name,
                            Duration = eff.Duration,
                            Behavior = eff.GetBehavior().Identifier(),
                            X = (eff is LocalFieldEffect ? (eff as LocalFieldEffect).X : 0),
                            Y = (eff is LocalFieldEffect ? (eff as LocalFieldEffect).Y : 0),
                            Radius = (eff is LocalFieldEffect ? (eff as LocalFieldEffect).Radius : 0),

                        })
                        .ToList();

                    await SendAmbientEffects(game);

                    await _hubContext.Clients.Group(game.Room.RoomCode).SendAsync("UpdateGameState",
                        player1.X, player1.Y,
                        player2.X, player2.Y,
                        game.Room.Puck.X, game.Room.Puck.Y, sentWalls, activeEffects, activePowerups, playerActivePowerups);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"Error in game loop: {ex.Message}");
                }
            }
        }

        private async Task SendAmbientEffects(Game game)
        {
            // pridet efektam priority, apsisieit su vienu iterator instance, padaryt foreachus vietoj while'u

            var lightingIterator = game.LightingEffects.CreateIterator();
            var particleIterator = game.ParticleEffects.CreateIterator();
            var soundIterator = game.SoundEffects.CreateIterator();

            while (lightingIterator.HasNext())
            {
                LightingEffect effect = lightingIterator.Next();
                await _hubContext.Clients.Group(game.Room.RoomCode).SendAsync("ShowLightingEffect",
                    effect.Area.X, effect.Area.Y, effect.Area.Width, effect.Area.Height,
                    effect.Duration,
                    effect.Color.ToArgb());

                lightingIterator.Remove(effect);
            }

            while (particleIterator.HasNext())
            {
                ParticleEffect effect = particleIterator.Next();
                await _hubContext.Clients.Group(game.Room.RoomCode).SendAsync("ShowParticleEffect",
                    effect.Type, effect.Position.X, effect.Position.Y,
                    effect.Lifetime);

                particleIterator.Remove(effect);
            }

            while (soundIterator.HasNext())
            {
                SoundEffect effect = soundIterator.Next();
                if (effect == null) continue;
                await _hubContext.Clients.Group(game.Room.RoomCode).SendAsync("PlaySoundEffect",
                    effect.Type, effect.Volume);
            }
        }

        private static Random random = new Random();
        public void GenerateWalls(Room room)
        {
            int numberOfWalls = random.Next(3, 6);

            AbstractWallFactory abstractWallFactory;

            List<Wall> wallsToAdd = new List<Wall>();

            int teleporterCount = 0;

            for (int i = 0; i < numberOfWalls;)
            {
                Wall? wall = null;
                bool isValidPosition = false;

                while (!isValidPosition)
                {
                    float x = (float)(35 + random.NextDouble() * (705 - 35));
                    float y = (float)(20 + random.NextDouble() * (391 - 20));
                    float width = (float)(10 + random.NextDouble() * (150 - 10));
                    float height = width > 75 ? (float)(10 + random.NextDouble() * (75 - 10)) : (float)(75 + random.NextDouble() * (150 - 75));
                    int wallType = random.Next(1, 9);

                    isValidPosition = true;

                    switch (wallType)
                    {
                        case 1:
                            if (IsPositionValid(wall, room, wallsToAdd)) { }
                            if (IsPositionValid(wall, room, wallsToAdd)) { }
                            break;
                    }
                    
                    switch (wallType)
                    {
                        case 1:
                            if (teleporterCount != 0)  // Only allow 2 teleporters
                            {
                                continue;
                            }

                            // Teleporter 1
                            abstractWallFactory = new StaticWallFactory();

                            wall = abstractWallFactory.CreateWall(i, width, height, "Teleporting", x, y);

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
                                    wall2 = abstractWallFactory.CreateWall(i, width2, height2, "Teleporting", x2, y2);

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
                            abstractWallFactory = new StaticWallFactory();
                            wall = abstractWallFactory.CreateWall(i, width, height, "QuickSand", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 3:
                            abstractWallFactory = new StaticWallFactory();
                            wall = abstractWallFactory.CreateWall(i, width, height, "Standard", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 4:
                            abstractWallFactory = new StaticWallFactory();
                            wall = abstractWallFactory.CreateWall(i, width, height, "Bouncy", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 5:
                            abstractWallFactory = new DynamicWallFactory();
                            wall = abstractWallFactory.CreateWall(i, width, height, "Bouncy", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 6:
                            abstractWallFactory = new DynamicWallFactory();
                            wall = abstractWallFactory.CreateWall(i, width, height, "Scrolling", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 7:
                            abstractWallFactory = new StaticWallFactory();
                            wall = abstractWallFactory.CreateWall(i, width, height, "Undo", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 8:
                            abstractWallFactory = new DynamicWallFactory();
                            wall = abstractWallFactory.CreateWall(i, width, height, "Breaking", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
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
        public void SpawnPowerups(Room room)
        {
            facade.SpawnPowerups(room);
        }
    }
}
