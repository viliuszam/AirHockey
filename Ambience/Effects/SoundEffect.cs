namespace AirHockey.Ambience.Effects
{
    public enum SoundType
    {
        Collision,
        GoalScored,
        PowerupActivated,
        GameStart,
        GameEnd
    }

    public class SoundEffect
    {
        public SoundType Type { get; set; }
        public float Volume { get; set; }

        public SoundEffect(SoundType type, float volume)
        {
            Type = type;
            Volume = volume;
        }
    }
}
