using AirHockey.Actors;
using AirHockey.Analytics;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;

namespace AirHockey.Observers
{
    public class ScoreObserver : IGoalObserver
    {
        private readonly IGameAnalytics _analytics;
        private readonly IHubContext<GameHub> _hubContext;

        public ScoreObserver(IGameAnalytics analytics, IHubContext<GameHub> hubContext) 
        {
            _analytics = analytics;
            _hubContext = hubContext; 
        }

        public async void OnGoalScored(Player scorer, Game game)
        {
            string roomCode = game.Room.RoomCode;

            
            _analytics.LogEvent(roomCode, "GoalScored", new Dictionary<string, object>
            {
                { "ScoringPlayer", scorer.Nickname },
                { "Score", $"{game.Player1Score} - {game.Player2Score}" },
                { "TimeStamp", DateTime.Now }
            });

            
            await _hubContext.Clients.Group(roomCode).SendAsync("GoalScored",
                scorer.Nickname, game.Player1Score, game.Player2Score);

            
            Console.WriteLine($"{scorer.Nickname} scored! Score is now {game.Player1Score} - {game.Player2Score}");
        }
    }
}
