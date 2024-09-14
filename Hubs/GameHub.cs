﻿using AirHockey.Actors;
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

    public async Task CreateRoom(string roomCode)
    {
        if (!_gameService.RoomExists(roomCode))
        {
            var room = new Room(roomCode);
            room.Players.Add(Context.ConnectionId);
            _gameService.AddRoom(room);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            Console.WriteLine($"Room {roomCode} created and Player 1 joined");

            await Clients.Caller.SendAsync("AssignPlayer", "Player1");

            await Clients.Caller.SendAsync("WaitingForPlayer");
        }
        else
        {
            await Clients.Caller.SendAsync("RoomCreationFailed", "Room already exists.");
        }
    }

    public async Task JoinRoom(string roomCode)
    {
        var room = _gameService.GetRoom(roomCode);
        if (room != null)
        {
            if (room.Players.Count >= 2)
            {
                await Clients.Caller.SendAsync("RoomFull", "Room is full. You cannot join.");
                return;
            }

            string player = room.Players.Count == 0 ? "Player1" : "Player2";
            room.Players.Add(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            Console.WriteLine($"{player} joined room {roomCode}");

            await Clients.Caller.SendAsync("AssignPlayer", player);

            if (room.Players.Count == 2)
            {
                _gameService.CreateGame(room);
                await Clients.Group(roomCode).SendAsync("StartGame");
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

        var game = _gameService.GetGame(roomCode);
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
        foreach (var room in _gameService.GetRooms().Values)
        {
            if (room.Players.Contains(Context.ConnectionId))
            {
                room.Players.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.RoomCode);
                if (room.Players.Count == 0)
                {
                    _gameService.RemoveRoom(room.RoomCode);
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
