/*using AirHockey.Actors;
using NAudio.Wave;

namespace AirHockey.Observers
{
    public class SoundEffectObserver : IGoalObserver
    {
        private readonly string _soundFilePath;

        
        public SoundEffectObserver(string soundFilePath)
        {
            _soundFilePath = soundFilePath;
        }

        public void OnGoalScored(Player scorer, Game game)
        {
            if (!System.IO.File.Exists(_soundFilePath))
            {
                throw new System.IO.FileNotFoundException($"Sound file not found: {_soundFilePath}");
            }

            using (var mp3FileReader = new Mp3FileReader(_soundFilePath))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(mp3FileReader);
                outputDevice.Play();

                // Wait until the sound finishes playing
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
    }
}*/
