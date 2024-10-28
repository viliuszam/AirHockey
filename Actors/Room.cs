﻿using AirHockey.Actors.Walls;
using AirHockey.Actors.Powerups;

namespace AirHockey.Actors
{
    public class Room
    {
        public string RoomCode { get; private set; }
        public List<Player> Players { get; set; }
        public List<Wall> Walls { get; set; } = new List<Wall>();
        public List<Powerup> Powerups { get; set; } = new List<Powerup>();
        public Puck Puck { get; set; }

        public Room(string roomCode)
        {
            RoomCode = roomCode;
            Players = new List<Player>();
            Puck = new Puck();
        }

        public void AddPlayer(Player player)
        {
            if (Players.Count < 2)
            {
                Players.Add(player);
            }
            else
            {
                throw new InvalidOperationException("Room is already full.");
            }
        }

        public Player GetPlayerById(string connectionId)
        {
            return Players.FirstOrDefault(p => p.Id == connectionId);
        }

        public void RemovePlayer(string connectionId)
        {
            var player = GetPlayerById(connectionId);
            if (player != null)
            {
                Players.Remove(player);
            }
        }

        public bool IsRoomFull()
        {
            return Players.Count >= 2;
        }
    }
}
