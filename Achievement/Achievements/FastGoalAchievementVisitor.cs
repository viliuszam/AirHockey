using AirHockey.Actors;
using Microsoft.AspNetCore.SignalR;

namespace AirHockey.Achievement.Achievements
{
    public class FastGoalAchievementVisitor : IAchievementVisitor
    {
        private IHubContext<GameHub> _hubContext;
        private const float FAST_GOAL_THRESHOLD_SECONDS = 10f;

        public FastGoalAchievementVisitor(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public async void Visit(Room room)
        {
            float timeSinceLastReset = room.Puck.TimeSinceReset;

            Console.WriteLine("time " + timeSinceLastReset);
            if (timeSinceLastReset <= FAST_GOAL_THRESHOLD_SECONDS)
            {
                int lastScorer = room.GetLast();
                if (lastScorer != -1 && lastScorer < room.Players.Count)
                {
                    var scoringPlayer = room.Players[lastScorer];
                    await _hubContext.Clients.Group(room.RoomCode).SendAsync("Achievement",
                        $"Fast Goal for {scoringPlayer.Nickname}! " +
                    $"Scored in {timeSinceLastReset:F2} seconds.");
                    //Console.WriteLine($"Achievement Unlocked: Fast Goal for {scoringPlayer.Nickname}! " +
                    //                  $"Scored in {timeSinceLastReset:F2} seconds.");
                }
            }
            
            room.Puck.TimeSinceReset = 0;
        }
    }
}