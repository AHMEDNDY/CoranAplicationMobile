using CoranWarshSynchroniser.Models;
using Plugin.Maui.Audio;

namespace CoranWarshSynchroniser.Services
{
    public class AudioService
    {
        private  IAudioPlayer _player;

        public event Action<int> AyahChanged;

        private string _currentSurah;

        int _currentIndex;

        public AudioService()
        {
            _player = AudioManager.Current.CreatePlayer();
            _player.PlaybackEnded += OnEnded;
        }


        public async Task LoadSurahAudioAsync(string audioPath)
        {
            // éviter de recharger si déjà ouvert
            if (_currentSurah == audioPath && _player != null)
                return;

            _currentSurah = audioPath;

            var stream = await FileSystem.OpenAppPackageFileAsync(audioPath);

            _player?.Dispose();

            _player = AudioManager.Current.CreatePlayer(stream);
        }

        public void PlayAyah(Surah ayah)
        {
            if (_player == null)
                return;

            // aller au startTime de l’ayah
            //_player.Seek(ayah.Start);

            if (!_player.IsPlaying)
                _player.Play();
        }
      
        void OnEnded(object sender, EventArgs e)
        {
            AyahChanged?.Invoke(_currentIndex + 1);
        }

        public void Pause() => _player.Pause();

        public bool IsPlaying() => _player.IsPlaying;

        public TimeSpan GetDuration()
        { return TimeSpan.FromSeconds(_player.Duration);
        }
        public TimeSpan GetCurrentPosition() 
        { 
            return TimeSpan.FromSeconds(_player.CurrentPosition); 
        }

        public Task SeekToAsync(TimeSpan position)
        {
            if (_player != null)
            {
                _player.Seek(position.TotalSeconds);
            }

            return Task.CompletedTask;
        }

        public void ResumeFrom(double seconds)
        {
            if (_player == null)
                return;

            _player.Seek(seconds);

            _player.Play();
        }

        public async Task ResumeFromAsync(double seconds)
        {
            if (_player == null)
                return;

            _player.Pause();
            await Task.Delay(100);

            _player.Seek(seconds); // ✅ IAudioPlayer utilise Seek(double), pas SeekTo(TimeSpan)
            await Task.Delay(150);

            _player.Play();
        }

        public void Stop()
        {
            if (_player == null) return;
            _player.Pause();
            _player.Seek(0);
        }
    }
}
