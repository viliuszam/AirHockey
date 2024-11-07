using AirHockey.Effects;
using AirHockey.Observers;

namespace AirHockey.Actors
{
    public class Game
    {
        public Room Room { get; private set; }
        public bool IsInitialized { get; set; } = false;

        public List<EnvironmentalEffect> ActiveEffects;

        public Game(Room room)
        {
            Room = room;
            ActiveEffects = new List<EnvironmentalEffect>();
        }
        public void StartGame()
        {
            Room.Player1Score = 0;
            Room.Player2Score = 0;
        }
    }
}
