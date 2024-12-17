using AirHockey.Actors;
using Microsoft.AspNetCore.SignalR;

namespace AirHockey.Achievement.Achievements
{
    public class GoalStreakAchievementVisitor : IAchievementVisitor
    {
        private IHubContext<GameHub> _hubContext;
        private const int STREAK_THRESHOLD = 3;
        private static Dictionary<string, int> playerGoalStreaks = new Dictionary<string, int>();

        public GoalStreakAchievementVisitor(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public async void Visit(Player player)
        {
            int lastScorer = player.Room.GetLast();
            
            int playerIndex = player.Room.Players.IndexOf(player);

            if (lastScorer == playerIndex)
            {
                if (!playerGoalStreaks.ContainsKey(player.Id))
                {
                    playerGoalStreaks[player.Id] = 1;
                }
                else
                {
                    playerGoalStreaks[player.Id]++;
                }

                var p1 = player.Room.Players[0];
                var p2 = player.Room.Players[1];
                playerGoalStreaks[lastScorer == 0 ? p2.Id : p1.Id] = 0;
                
                if (playerGoalStreaks[player.Id] >= STREAK_THRESHOLD)
                {
                    //Console.WriteLine($"Achievement Unlocked: Goal Streak for {player.Nickname}!");
                    await _hubContext.Clients.Group(player.Room.RoomCode).SendAsync("Achievement",
                        $"Goal Streak for {player.Nickname}! ");
                    playerGoalStreaks[player.Id] = 0;
                }
            }
        }

    }
}