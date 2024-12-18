using AirHockey.Actors;
using AirHockey.Ambience.Effects;
using AirHockey.Managers;
using AirHockey.Services;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics.CodeAnalysis;
using AirHockey.Handlers;


[ExcludeFromCodeCoverage]
public class GameHub : Hub
{
    private readonly GameService _gameService;

    public GameHub(GameService gameService)
    {
        _gameService = gameService;
    }

    public async Task CreateRoom(string roomCode, string nickname)
    {
        if (!GameSessionManager.Instance.RoomExists(roomCode))
        {
            var room = new Room(roomCode);
            var player = new Player(Context.ConnectionId, "red", 227, 260, nickname, room);
            room.AddPlayer(player);

            GameSessionManager.Instance.AddRoom(room);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Caller.SendAsync("AssignPlayer", Context.ConnectionId, nickname);
            await Clients.Caller.SendAsync("WaitingForPlayer");
            room.SetState(Room.RoomState.Waiting);

            var game = GameSessionManager.Instance.GetGame(roomCode);
            if (game != null)
            {
                await Clients.Caller.SendAsync("UpdateScores", game.Room.Player1Score, game.Room.Player2Score);
            }
        }
        else
        {
            await Clients.Caller.SendAsync("RoomCreationFailed", "Room " + roomCode  + " already exists.");
        }
    }

    public async Task JoinRoom(string roomCode, string nickname)
    {
        var room = GameSessionManager.Instance.GetRoom(roomCode);
        if (room != null)
        {
            if (room.IsRoomFull())
            {
                await Clients.Caller.SendAsync("RoomFull", "Room is full. You cannot join.");
                return;
            }

            var player = new Player(Context.ConnectionId, "blue", 633, 260, nickname, room);
            room.AddPlayer(player);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Caller.SendAsync("AssignPlayer", Context.ConnectionId, nickname);

            if (room.IsRoomFull())
            {
                GameSessionManager.Instance.StartNewGame(room);

                string player1Nickname = room.Players[0].Nickname;
                string player2Nickname = room.Players[1].Nickname;

                var game = GameSessionManager.Instance.GetGame(roomCode);

                _gameService.GenerateWalls(room);
                foreach (var wall in room.Walls)
                {
                    await Clients.Group(roomCode).SendAsync("AddWall", wall.Id, wall.X, wall.Y, wall.Width, wall.Height, wall.GetType().Name);
                }
                _gameService.SpawnPowerups(room);

                foreach (var powerup in _gameService.GetPowerupsByRoom(room))
                {
                    Console.WriteLine($"Powerup id: {powerup.Id}");
                    Console.WriteLine($"X: {powerup.X}, Y: {powerup.Y}");
                    await Clients.Group(roomCode).SendAsync("AddPowerup", powerup.Id, powerup.X, powerup.Y, powerup.GetBaseType().Name, powerup.IsActive);
                }
                await Clients.Group(roomCode).SendAsync("StartGame",
                    player1Nickname, player2Nickname, game.Room.Player1Score, game.Room.Player2Score);
                room.SetState(Room.RoomState.Playing);

                game.SoundEffects.AddEffect(new SoundEffect(SoundType.GameStart, 0.2f));
            }
        }
        else
        {
            await Clients.Caller.SendAsync("RoomNotFound", "The room you are trying to join (" + roomCode + ") does not exist.");
        }
    }

    public async Task UpdateInput(string roomCode, string connectionId, bool up, bool down, bool left, bool right, bool powerup, bool pause)
    {
        //Console.WriteLine($"Received input from {connectionId} in room {roomCode}: Up={up}, Down={down}, Left={left}, Right={right}, Powerup={powerup}");

        var game = GameSessionManager.Instance.GetGame(roomCode);
        if (game == null) return;

        var player = game.Room.GetPlayerById(connectionId);
        if (player == null) return;

        var inputs = new Dictionary<string, bool>
        {
            { "up", up },
            { "down", down },
            { "left", left },
            { "right", right },
            { "powerup", powerup },
            { "pause", pause }
        };

        var context = new InputContext
        {
            RoomCode = roomCode,
            ConnectionId = connectionId,
            Inputs = inputs,
            Game = game,
            Player = player
        };

        if (pause)
        {
            await Clients.Group(roomCode).SendAsync("TogglePause", connectionId, player.Nickname);
        }
        
        var movementHandler = new MovementHandler();
        var powerupHandler = new PowerupHandler();
        var pauseHandler = new PauseHandler();
        
        movementHandler
            .SetNext(powerupHandler)
            .SetNext(pauseHandler);
        
        movementHandler.Handle(context);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        foreach (var room in GameSessionManager.Instance.ActiveRooms.Values)
        {
            if (room.Players.Any(p => p.Id == Context.ConnectionId))
            {
                var player = room.GetPlayerById(Context.ConnectionId);
                if (player != null)
                {
                    room.RemovePlayer(Context.ConnectionId);

                    if (room.Players.Count > 0)
                    {
                        await Clients.Group(room.RoomCode).SendAsync("PlayerDisconnected", "Opponent has disconnected. Game canceled.");
                    }

                    GameSessionManager.Instance.RemoveRoom(room.RoomCode);

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.RoomCode);

                    Console.WriteLine($"Room {room.RoomCode} disbanded due to player disconnection.");

                    break;
                }
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task ProcessCommand(string roomCode, string connectionId, string command)
    {
        var game = GameSessionManager.Instance.GetGame(roomCode);
        if (game == null) return;
        var player = game.Room.GetPlayerById(connectionId);
        if (player == null) return;
        _gameService.ProcessCommand(player, command);
        if (command.StartsWith("setscore"))
        {
            var player1Score = game.Room.Player1Score;
            var player2Score = game.Room.Player2Score;
            await Clients.Group(roomCode).SendAsync("UpdateScores", player1Score, player2Score);
        }
    }
    
}
