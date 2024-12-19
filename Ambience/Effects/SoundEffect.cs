namespace AirHockey.Ambience.Effects
{
    public enum SoundType
    {
        // ids used in mp3 names
        GoalScored = 1,
        PowerupActivated = 2,
        Collision = 3,
        GameStart = 4
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
