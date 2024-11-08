using AirHockey.Actors.Powerups.PowerupDecorators;
using AirHockey.Actors.Powerups;
using AirHockey.Actors;
using AirHockey.Actors.Walls;
using System.Drawing;
using AirHockey.Strategies;
using AirHockey.Actors.Command;

namespace AirHockey.Facades
{
    public class Facade
    {
        private static Random random;
        private List<ICommand> commandLists = new List<ICommand>();
        private ICollision collisions;
        public Facade()
        {
            random = new Random();
        }
        public void InitializeCommands(Game game)
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
        public void TrackEntityMovement()
        {
            foreach (ICommand commandList in commandLists)
            {
                commandList.Execute();
            }
        }
        public void SpawnPowerups(Room room)
        {
            int numberOfPowerups = random.Next(2, 5);
            PowerupFactory powerupFactory = new PowerupFactory();

            Powerup dashPrototype = powerupFactory.CreatePowerup(0, 0, 1, "Dash");
            // CloneShallow comparison
            Powerup dashShallowClone = dashPrototype.CloneShallow();
            Console.WriteLine($"Dash Prototype Hash Code: {dashPrototype.GetHashCode()}");
            Console.WriteLine($"Dash Shallow Clone Hash Code: {dashShallowClone.GetHashCode()}");

            // CloneDeep comparison
            Powerup dashDeepClone = dashPrototype.CloneDeep();
            Console.WriteLine($"Dash Deep Clone Hash Code: {dashDeepClone.GetHashCode()}");

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
        public IEnumerable<dynamic> GetActivePowerups(Game game)
        {
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

            return activePowerups;
        }
        public List<string> GetPlayerActivePowerups(Player player1, Player player2)
        {
            var playerActivePowerups = new List<string>
            {
                player1.ActivePowerup?.GetBaseType().Name ?? string.Empty,
                player2.ActivePowerup?.GetBaseType().Name ?? string.Empty
            };

            return playerActivePowerups;
        }
        public List<Powerup> GetAllPowerupsByRoom(Room room)
        {
            return room.Powerups.ToList();
        }
        public void SetStrategy(ICollision newCollisionStrategy)
        {
            collisions = newCollisionStrategy;
        }
        public void HandleCollisions(Game game)
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
                collisions.ResolveCollision(player2, puck);
            }
            foreach (var wall in game.Room.Walls)
            {
                if (wall is QuickSandWall) SetStrategy(new QuickCollision());
                else if (wall is TeleportingWall) SetStrategy(new TeleportCollision());
                else if (wall is ScrollingWall) SetStrategy(new ScrolingCollision());
                else if (wall is BouncyWall) SetStrategy(new BouncyCollision());
                else if (wall is BreakingWall) SetStrategy(new BreakingColision());

                else if (wall is UndoWall) SetStrategy(new UndoCollision());

                else SetStrategy(new WallCollision());
                if (wall.IsColliding(player1))
                {
                    if (wall is UndoWall undoWall)
                    {
                        if (undoWall.isActive())
                        {
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
                        if (undoWall.isActive())
                        {
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
                        if (undoWall.isActive())
                        {
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
                    else if (wall is BreakingWall) SetStrategy(new BreakingColision());

                    else SetStrategy(new WallCollision());
                    if (wall != otherWall && wall.IsColliding(otherWall))
                    {
                        collisions.ResolveCollision(wall, otherWall);
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
        public void UndoOnCollision(Entity entity)
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
    }
}
