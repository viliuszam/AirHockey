using AirHockey.Actors;
using Microsoft.AspNetCore.SignalR;

namespace AirHockey.Achievement.Achievements
{
    public class LongDistanceGoalAchievementVisitor : IAchievementVisitor
    {
        private IHubContext<GameHub> _hubContext;
        private const float LONG_DISTANCE_THRESHOLD = 200f;

        public LongDistanceGoalAchievementVisitor(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public async void Visit(Player player)
        {
            int lastScorer = player.Room.GetLast();
            
            int playerIndex = player.Room.Players.IndexOf(player);

            if (lastScorer == playerIndex)
            {
                float goalDistance = player.DistanceToGoalLastCollision;

                if (goalDistance >= LONG_DISTANCE_THRESHOLD)
                {
                    //Console.WriteLine($"Achievement Unlocked:  " +
                     //                 $"Distance: {goalDistance:F2}");
                     await _hubContext.Clients.Group(player.Room.RoomCode).SendAsync("Achievement",
                         $"Long Distance Goal for {player.Nickname}! Distance: {goalDistance:F2}");
                }
            }
        }
    }
}