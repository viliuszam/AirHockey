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

namespace AirHockey.Services
{
    [ExcludeFromCodeCoverage]
    public class GameService
    {
        private readonly IGameAnalytics _analytics;
        private readonly IHubContext<GameHub> _hubContext;
        private System.Timers.Timer gameLoopTimer;
        private ICollision collisions;
        private const float MIN_X = 0f; 
        private const float MAX_X = 855f;
        private const float MIN_Y = 0f;
        private const float MAX_Y = 541f;

        private const float GOAL_WIDTH = 25f;
        private const float GOAL_Y_MIN = 180f;
        private const float GOAL_Y_MAX = 365f;

        private const int maxEffects = 5;

        private List<ICommand> commandLists = new List<ICommand>();

        public GameService(IHubContext<GameHub> hubContext, IGameAnalytics analytics, ICollision col)
        {
            _hubContext = hubContext;
            _analytics = analytics;
            gameLoopTimer = new System.Timers.Timer(16);  // 16*60 ~ apie 60 FPS
            gameLoopTimer.Elapsed += GameLoop;
            gameLoopTimer.Start();
            collisions = col;
        }
        public void SetStrategy(ICollision newCollisionStrategy)
        {
            collisions = newCollisionStrategy;
        }

        private void InitializeCommands(Game game)
        {

            commandLists.Clear(); 

            var player1 = game.Room.Players[0];
            var player2 = game.Room.Players[1];
            var puck = game.Room.Puck;

            var player1MoveCommand = new MoveCommand(player1, new Queue<(float X, float Y, float timestamp)>());
            var player2MoveCommand = new MoveCommand(player2, new Queue<(float X, float Y, float timestamp)>());
            var puckMoveCommand = new MoveCommand(puck, new Queue<(float X, float Y, float timestamp)>());

            commandLists.Add(player1MoveCommand);
            commandLists.Add(player2MoveCommand);
            commandLists.Add(puckMoveCommand);
        }

        public bool RoomExists(string room)
        {
            return GameSessionManager.Instance.RoomExists(room);
        }

        private void TrackEntityMovement()
        {
            foreach(ICommand commandList in commandLists){
                commandList.Execute();
            }
        }

        private void HandleCollisions(Game game)
        {
            var player1 = game.Room.Players[0];
            var player2 = game.Room.Players[1];
            var puck = game.Room.Puck;
            SetStrategy(new BaseCollision());
            if (player1.IsColliding(player2))
            {
                collisions.ResolveCollision(player1, player2);
            }
            if (player1.IsColliding(puck))
            {
                collisions.ResolveCollision(player1, puck);
            }
            if (player2.IsColliding(puck))
            {
                collisions.ResolveCollision(player2,puck);
            }
            foreach (var wall in game.Room.Walls)
            {
                if (wall is QuickSandWall) SetStrategy(new QuickCollision());
                else if (wall is TeleportingWall) SetStrategy(new TeleportCollision());
                else if (wall is ScrollingWall) SetStrategy(new ScrolingCollision());
                else if (wall is BouncyWall) SetStrategy(new BouncyCollision());
                else if (wall is UndoWall) SetStrategy(new UndoCollision());

                else SetStrategy(new WallCollision());
                if (wall.IsColliding(player1))
                {
                    if (wall is UndoWall undoWall)
                    {
                        if(undoWall.isActive()){
                            UndoOnCollision(player1);
                            undoWall.setInactive();
                        }
                    }
                    collisions.ResolveCollision(wall, player1);
                }

                if (wall.IsColliding(player2))
                {
                    if (wall is UndoWall undoWall)
                    {
                        if(undoWall.isActive()){
                            UndoOnCollision(player2);
                            undoWall.setInactive();
                        }
                    }
                    collisions.ResolveCollision(wall, player2);
                }

                if (wall.IsColliding(puck))
                {
                    if (wall is UndoWall undoWall)
                    {
                        if(undoWall.isActive()){
                            UndoOnCollision(puck);
                            undoWall.setInactive();
                        }
                    }
                    collisions.ResolveCollision(wall, puck);
                }

                foreach (var otherWall in game.Room.Walls)
                {
                    if (wall is QuickSandWall) SetStrategy(new QuickCollision());
                    else if (wall is TeleportingWall) SetStrategy(new TeleportCollision());
                    else if (wall is ScrollingWall) SetStrategy(new ScrolingCollision());
                    else if (wall is BouncyWall) SetStrategy(new BouncyCollision());
                    else if (wall is UndoWall) SetStrategy(new UndoCollision());

                    else SetStrategy(new WallCollision());
                    if (wall != otherWall && wall.IsColliding(otherWall))
                    {
                        collisions.ResolveCollision(wall,otherWall);
                    }
                }
            }

            foreach (var powerup in game.Room.Powerups)
            {
                if (powerup.IsColliding(player1))
                {
                    powerup.ResolveCollision(player1);
                }
                if (powerup.IsColliding(player2))
                {
                    powerup.ResolveCollision(player2);
                }
            }
            
        }

        private void UndoOnCollision(Entity entity)
        {
            foreach (var command in commandLists)
            {
                if (command is MoveCommand moveCommand && moveCommand.getEntity() == entity)
                {
                    moveCommand.Undo();
                    break;
                }
            }
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
            if (game.GetLast() != -1)
            {

                string roomCode = game.Room.RoomCode;

                await _hubContext.Clients.Group(roomCode).SendAsync("GoalScored",
                    game.Room.Players[game.GetLast()].Nickname, game.Player1Score, game.Player2Score);

                // log goal
                _analytics.LogEvent(roomCode, "GoalScored", new Dictionary<string, object>
                {
                    { "ScoringPlayer", game.Room.Players[game.GetLast()].Nickname },
                    { "Score", $"{game.Player1Score} - {game.Player2Score}" },
                    { "TimeStamp", DateTime.Now }
                });

                Console.WriteLine($"{game.Room.Players[game.GetLast()].Nickname} scored! Score is now {game.Player1Score} - {game.Player2Score}");
                game.SetLast();
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
                        game.Room.Puck.Attach(game);
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

                    HandleCollisions(game);
                    CheckGoal(game);

                    player1.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    player2.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);
                    puck.ConstrainToBounds(MIN_X, MIN_Y, MAX_X, MAX_Y);


                    var activePowerups = game.Room.Powerups
                        .Select(powerup => new
                        {
                            powerup.Id,
                            powerup.X,
                            powerup.Y,
                            powerup.GetBaseType().Name,
                            powerup.IsActive
                        })
                        .ToList();

                    var playerActivePowerups = new List<string>
                    {
                        player1.ActivePowerup?.GetBaseType().Name ?? string.Empty,
                        player2.ActivePowerup?.GetBaseType().Name ?? string.Empty
                    };

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
                    int wallType = random.Next(1, 8);

                    isValidPosition = true;

                    switch (wallType)
                    {
                        case 1:
                            if (teleporterCount != 0)  // Only allow 2 teleporters
                            {
                                continue;
                            }

                            // Teleporter 1
                            abstractWallFactory = AbstractWallFactory.GetFactory(isDynamic: false);

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
                            abstractWallFactory = AbstractWallFactory.GetFactory(isDynamic: false);
                            wall = abstractWallFactory.CreateWall(i, width, height, "QuickSand", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 3:
                            abstractWallFactory = AbstractWallFactory.GetFactory(isDynamic: false);
                            wall = abstractWallFactory.CreateWall(i, width, height, "Standard", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 4:
                            abstractWallFactory = AbstractWallFactory.GetFactory(isDynamic: false);
                            wall = abstractWallFactory.CreateWall(i, width, height, "Bouncy", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 5:
                            abstractWallFactory = AbstractWallFactory.GetFactory(isDynamic: true);
                            wall = abstractWallFactory.CreateWall(i, width, height, "Bouncy", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 6:
                            abstractWallFactory = AbstractWallFactory.GetFactory(isDynamic: true);
                            wall = abstractWallFactory.CreateWall(i, width, height, "Scrolling", x, y);
                            if (IsPositionValid(wall, room, wallsToAdd))
                            {
                                wallsToAdd.Add(wall);
                                i++;
                            }
                            break;
                        case 7:
                            abstractWallFactory = AbstractWallFactory.GetFactory(isDynamic: false);
                            wall = abstractWallFactory.CreateWall(i, width, height, "Undo", x, y);
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

        public void SpawnPowerups(Room room)
        {
            int numberOfPowerups = random.Next(2, 5);
            PowerupFactory powerupFactory = new PowerupFactory();

            Powerup dashPrototype = powerupFactory.CreatePowerup(0, 0, 1, "Dash");
            Powerup freezePrototype = powerupFactory.CreatePowerup(0, 0, 2, "Freeze");
            Powerup pushPrototype = powerupFactory.CreatePowerup(0, 0, 3, "Push");

            List<Powerup> powerupsToAdd = new List<Powerup>();

            for (int i = 0; i < numberOfPowerups; i++)
            {
                Powerup powerup = null;
                bool isValidPosition = false;

                while (!isValidPosition)
                {
                    float x = (float)(50 + random.NextDouble() * (705 - 50));
                    float y = (float)(50 + random.NextDouble() * (391 - 50));

                    string powerupType = GetRandomPowerupType();

                    switch (powerupType)
                    {
                        case "Dash":
                            powerup = dashPrototype.CloneDeep();
                            break;
                        case "Freeze":
                            powerup = freezePrototype.CloneDeep();
                            break;
                        case "Push":
                            powerup = pushPrototype.CloneDeep();
                            break;
                    }

                    powerup.X = x;
                    powerup.Y = y;
                    powerup.Id = i + 1;

                    powerup = ApplyRandomDecorators(powerup);

                    if (IsPowerupPositionValid(powerup, room, powerupsToAdd))
                    {
                        powerupsToAdd.Add(powerup);
                        isValidPosition = true;
                    }
                }
            }

            foreach (var newPowerup in powerupsToAdd)
            {
                room.Powerups.Add(newPowerup);
            }
        }
        private Powerup ApplyRandomDecorators(Powerup powerup)
        {
            float duration = 5f; // Duration for the effects of the decorators

            // Example: randomly decide to apply speed multiplier, mass multiplier, or acceleration multiplier
            if (random.NextDouble() < 0.5) // 50% chance to apply a speed multiplier
            {
                float speedMultiplier = random.Next(2, 4); // Random multiplier between 2 and 4
                powerup = new SpeedMultiplierDecorator(powerup, speedMultiplier, duration);
            }
            if (random.NextDouble() < 0.5) // 50% chance to apply a mass multiplier
            {
                float massMultiplier = random.Next(2, 4); // Random multiplier between 2 and 4
                powerup = new MassMultiplierDecorator(powerup, massMultiplier, duration);
            }
            if (random.NextDouble() < 0.5) // 50% chance to apply an acceleration multiplier
            {
                float accelerationMultiplier = random.Next(2, 4); // Random multiplier between 2 and 4
                powerup = new AccelerationMultiplierDecorator(powerup, accelerationMultiplier, duration);
            }

            return powerup;
        }
        private string GetRandomPowerupType()
        {
            string[] powerupTypes = { "Dash", "Freeze", "Push" };
            return powerupTypes[random.Next(powerupTypes.Length)];
        }

        private bool IsPowerupPositionValid(Powerup powerup, Room room, List<Powerup> powerupsToAdd)
        {
            var exclusionZones = new List<Rectangle>
            {
                new Rectangle(175, 220, 100, 100),  // Player 1
                new Rectangle(575, 220, 100, 100),  // Player 2
                new Rectangle(375, 220, 100, 100)   // Puck
            };


            foreach (var exclusionZone in exclusionZones)
            {
                if (powerup.IsColliding(exclusionZone))
                {
                    return false;
                }
            }

            // Check if the powerup is colliding with existing walls
            foreach (var wall in room.Walls)
            {
                if (powerup.IsColliding(wall))
                {
                    return false;
                }
            }

            // Check if the powerup is colliding with any other powerups
            foreach (var existingPowerup in powerupsToAdd)
            {
                if (powerup.IsColliding(existingPowerup))
                {
                    return false;
                }
            }

            return true;
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
