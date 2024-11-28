using AirHockey.Ambience.Collections;
using AirHockey.Ambience.Effects;
using AirHockey.Effects;
using AirHockey.Observers;

namespace AirHockey.Actors
{
    public class Game
    {
        public Room Room { get; private set; }
        public bool IsInitialized { get; set; } = false;

        public List<EnvironmentalEffect> ActiveEffects;

        public LightingEffectCollection LightingEffects { get; } = new LightingEffectCollection();
        public SoundEffectCollection SoundEffects { get; } = new SoundEffectCollection();
        public ParticleEffectCollection ParticleEffects { get; } = new ParticleEffectCollection();


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
