using AirHockey.Actors;
using AirHockey.Managers;
using AirHockey.Services;
using Microsoft.AspNetCore.SignalR;

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
            var player = new Player(Context.ConnectionId, "red", 227, 260, nickname);
            room.AddPlayer(player);

            GameSessionManager.Instance.AddRoom(room);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Caller.SendAsync("AssignPlayer", Context.ConnectionId, nickname);
            await Clients.Caller.SendAsync("WaitingForPlayer");

            var game = GameSessionManager.Instance.GetGame(roomCode);
            if (game != null)
            {
                await Clients.Caller.SendAsync("UpdateScores", game.Player1Score, game.Player2Score);
            }
        }
        else
        {
            await Clients.Caller.SendAsync("RoomCreationFailed", "Room already exists.");
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

            var player = new Player(Context.ConnectionId, "blue", 633, 260, nickname);
            room.AddPlayer(player);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Caller.SendAsync("AssignPlayer", Context.ConnectionId, nickname);

            if (room.IsRoomFull())
            {
                GameSessionManager.Instance.StartNewGame(room);

                string player1Nickname = room.Players[0].Nickname;
                string player2Nickname = room.Players[1].Nickname;

                var game = GameSessionManager.Instance.GetGame(roomCode);

                await Clients.Group(roomCode).SendAsync("StartGame",
                    player1Nickname, player2Nickname, game.Player1Score, game.Player2Score);
            }
        }
        else
        {
            await Clients.Caller.SendAsync("RoomNotFound", "The room you are trying to join does not exist.");
        }
    }

    public async Task UpdateInput(string roomCode, string connectionId, bool up, bool down, bool left, bool right)
    {
        Console.WriteLine($"Received input from {connectionId} in room {roomCode}: Up={up}, Down={down}, Left={left}, Right={right}");

        var game = GameSessionManager.Instance.GetGame(roomCode);
        if (game != null)
        {
            var player = game.Room.GetPlayerById(connectionId);

            if (player != null)
            {
                float xDirection = (left ? -1 : 0) + (right ? 1 : 0);
                float yDirection = (up ? -1 : 0) + (down ? 1 : 0);

                Console.WriteLine($"Player {connectionId} accelerates in direction: ({xDirection}, {yDirection})");

                player.Accelerate(xDirection, yDirection);
            }
        }
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
}
