using AirHockey.Actors;
using AirHockey.Managers;
using AirHockey.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

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
            room.Players.Add(Context.ConnectionId);

            GameSessionManager.Instance.AddPlayerNickname(Context.ConnectionId, nickname);
            GameSessionManager.Instance.AddRoom(room);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Caller.SendAsync("AssignPlayer", "Player1", nickname);
            await Clients.Caller.SendAsync("WaitingForPlayer");
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
            if (room.Players.Count >= 2)
            {
                await Clients.Caller.SendAsync("RoomFull", "Room is full. You cannot join.");
                return;
            }

            string player = room.Players.Count == 0 ? "Player1" : "Player2";
            room.Players.Add(Context.ConnectionId);

            GameSessionManager.Instance.AddPlayerNickname(Context.ConnectionId, nickname);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Caller.SendAsync("AssignPlayer", player, nickname);

            if (room.Players.Count == 2)
            {
                GameSessionManager.Instance.StartNewGame(room);

                string player1Nickname = GameSessionManager.Instance.GetPlayerNickname(room.Players[0]);
                string player2Nickname = GameSessionManager.Instance.GetPlayerNickname(room.Players[1]);

                await Clients.Group(roomCode).SendAsync("StartGame", player1Nickname, player2Nickname);
            }
        }
        else
        {
            await Clients.Caller.SendAsync("RoomNotFound", "The room you are trying to join does not exist.");
        }
    }

    public async Task UpdateInput(string roomCode, string playerId, bool up, bool down, bool left, bool right)
    {
        Console.WriteLine($"Received input from {playerId} in room {roomCode}: Up={up}, Down={down}, Left={left}, Right={right}");

        var game = GameSessionManager.Instance.GetGame(roomCode);
        if (game != null)
        {
            var player = playerId == "Player1" ? game.Player1 : game.Player2;

            float xDirection = (left ? -1 : 0) + (right ? 1 : 0);
            float yDirection = (up ? -1 : 0) + (down ? 1 : 0);

            Console.WriteLine($"Player {playerId} accelerates in direction: ({xDirection}, {yDirection})");

            player.Accelerate(xDirection, yDirection);
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        foreach (var room in GameSessionManager.Instance.ActiveRooms.Values)
        {
            if (room.Players.Contains(Context.ConnectionId))
            {
                room.Players.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.RoomCode);

                if (room.Players.Count == 0)
                {
                    GameSessionManager.Instance.RemoveRoom(room.RoomCode);
                    Console.WriteLine($"Room {room.RoomCode} deleted as no players remain.");
                }
                else
                {
                    await Clients.Group(room.RoomCode).SendAsync("PlayerDisconnected", "Opponent has disconnected. Game canceled.");
                }
                break;
            }
        }
        await base.OnDisconnectedAsync(exception);
    }
}
