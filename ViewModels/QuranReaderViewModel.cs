using CoranWarshSynchroniser.Models;
using CoranWarshSynchroniser.Services;
using Microsoft.Maui.Primitives;
using MvvmHelpers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CoranWarshSynchroniser.ViewModels
{
    public class QuranReaderViewModel : BaseViewModel
    {
        #region Services 

        private readonly AudioService _audioService;

        #endregion

        #region Attributes

        public bool CanGoNext => _currentSurahNumber < 114;
        public bool IsLoading { get; set; }

        private string _name;
        private string _name_english;
        private bool _mecca;
        private int _currentSurahNumber;
        private int _totalverse;
        private Surah _currentAyah;
        private int _indexCurrentColoredAyah = 1;
        public int SourateId;
        private IDispatcherTimer _syncTimer;
        private bool _isPlayVisible = true;
        private bool _isPauseVisible = false;
 

        #endregion Attributes

        #region Commands

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand NextSurahCommand { get; }
        public ICommand GotoPageListeCommand { get; }
        public ICommand PreviousSurahCommand { get; }
        public ICommand TapCommand { get; }

        #endregion Commands

        #region Actions

        public Action<int>? ScrollToSpanRequested { get; set; }

        #endregion

        #region Properties


        private ObservableCollection<Surah> _surahJsonData;

        public ObservableCollection<Surah> SurahJsonData
        {
            get => _surahJsonData;
            set => SetProperty(ref _surahJsonData, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string NameEnglish
        {
            get => _name_english;
            set => SetProperty(ref _name_english, value);
        }
        public bool IsMecca
        {
            get => _mecca;
            set => SetProperty(ref _mecca, value);
        }
        public int Totalverse
        {
            get => _totalverse;
            set => SetProperty(ref _totalverse, value);
        }
        public bool IsPlayVisible
        {
            get => _isPlayVisible;
            set
            {
                _isPlayVisible = value;
                OnPropertyChanged();
            }
        }
        public bool IsPauseVisible
        {
            get => _isPauseVisible;
            set
            {
                _isPauseVisible = value;
                OnPropertyChanged();
            }
        }


        public int CurrentSurahNumber
        {
            get => _currentSurahNumber;
            set => SetProperty(ref _currentSurahNumber, value);
        }
        #endregion // end Properties

        #region Constructors


        public QuranReaderViewModel()
        {
            _audioService = new AudioService();

            PlayCommand = new Command(async () => await PlayCommandHandler());
            PauseCommand = new Command(() => PauseCommandHandler());
            NextSurahCommand = new Command(async () => await NextSurahCommandHandler());
            GotoPageListeCommand = new Command(async () => await GotoList());
            PreviousSurahCommand = new Command(async () => await PreviousSurahCommandHandler());
            TapCommand = new Command<int>(async ayaNo => await TapCommandHandler(ayaNo));
            _syncTimer = Application.Current.Dispatcher.CreateTimer(); 
            _syncTimer.Interval = TimeSpan.FromMilliseconds(80); 
            
            _syncTimer.Start();
            _syncTimer.Tick += SyncTick;
            //CurrentSurahNumber = 1;

            LoadSurah();
            
        }

        #endregion // end Constructors

        #region Public Methods
        public void LoadSourate(int id)
        {
            CurrentSurahNumber = id;
            //LoadSurahAsync(CurrentSurahNumber);
        }
      
        private void SyncTick(object sender, EventArgs e)
        {
            if (_currentAyah == null)
                return;

            double currentTime = _audioService.GetCurrentPosition().TotalSeconds;

            // passer au suivant uniquement si End atteint
            if (currentTime >= _currentAyah.End)
            {
                OnAyahChanged(_currentAyah.AyaNo + 1);
            }
        }

        public async Task LoadSurah()
        {
            IsLoading = true;
            OnPropertyChanged(nameof(IsLoading));
            using var stream = await FileSystem.OpenAppPackageFileAsync("surahs.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            SurahJsonData = new ObservableCollection<Surah>(
                    JsonConvert.DeserializeObject<List<Surah>>(json)
                    .Where(a => a.SuraNo == CurrentSurahNumber)
                    .OrderBy(a => a.AyaNo)
                    .ToList()
                );

            _currentAyah = SurahJsonData.First();
            Name = _currentAyah.SuraNameAr;
            NameEnglish = _currentAyah.SuraNameEn;
            Totalverse = _currentAyah.TotalVerses;
            IsMecca = _currentAyah.IsMecca;



            foreach (var ayah in SurahJsonData)
            {
                ayah.ShowTranslation = true;
                var span = new Span
                {
                    Text = ayah.AyaText,
                    FontFamily = "UthmanicWarsh",
                    FontSize = 20,
                    TextColor = Colors.Black
                };
                var tap = new TapGestureRecognizer()
                {
                    Command = TapCommand,
                    CommandParameter = ayah.AyaNo
                };

                span.GestureRecognizers.Add(tap);

                ayah.Translation = ayah.Translation ?? "";

            }

            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));
        }

        public async Task LoadSurahAsync(int surahNumber)
        {

            string audioFile = $"Audio/{surahNumber:D3}.mp3";
            await _audioService.LoadSurahAudioAsync(audioFile);
        }


        #endregion Public Methods

        #region Private Methods

        private void HighlightAyah(int index)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var ayah in SurahJsonData)
                {
                    //ayah.BackgroundColor = Color.FromArgb("#F8F8F8");

                    ayah.TextColor = Colors.Black;

                    ayah.TranslationColor = Color.FromArgb("#6E6E6E");

                    // reset background arabe
                    ayah.AyahBackgroundColor = Colors.Transparent;
                }

                var current = SurahJsonData.FirstOrDefault(a => a.AyaNo == index);

                if (current != null)
                {
                    // background seulement texte arabe
                    //current.AyahBackgroundColor = Color.FromArgb("#C8A951");

                    // texte arabe blanc
                    current.TextColor = Colors.Blue;

                    // traduction normale
                    current.TranslationColor = Color.FromArgb("#C8A951");
                }

                _indexCurrentColoredAyah = index;
            });
        }

        private async Task PlayCommandHandler()
        {
            LoadSurahAsync(CurrentSurahNumber);
            if (_audioService.IsPlaying())
                return;

            if (SurahJsonData == null || SurahJsonData.Count == 0)
                await LoadSurah();

            _currentAyah ??= SurahJsonData.First(a => a.AyaNo == 1);
            
            _audioService.PlayAyah(_currentAyah);
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                IsPlayVisible = false;
                IsPauseVisible = true;
            });


            OnAyahChanged(_currentAyah.AyaNo);

            _audioService.AyahChanged += OnAyahChanged;
      

            ScrollToSpanRequested?.Invoke(_currentAyah.AyaNo - 1);
        }

        private void PauseCommandHandler()
        {
            if (!_audioService.IsPlaying())
                return;

            _audioService.Pause();
            // ✅ Forcer la mise à jour sur le Main Thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsPauseVisible = false;
                IsPlayVisible = true;
            });

        }

        private async Task PreviousSurahCommandHandler()
        {
            if (_audioService.IsPlaying())
                _audioService.Stop();

            if (_currentSurahNumber > 1)
                _currentSurahNumber--;

            await LoadSurah();
            //LoadSurahAsync(CurrentSurahNumber);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsPlayVisible = true;
                IsPauseVisible = false;
            });
        }
        private async Task NextSurahCommandHandler()
        {
            if (_audioService.IsPlaying())
                _audioService.Stop();

            if (_currentSurahNumber >= 114) // sécurité
                return;

            _currentSurahNumber++;

            await LoadSurah();
            //LoadSurahAsync(CurrentSurahNumber);

            // Notifier le changement des boutons
            OnPropertyChanged(nameof(CanGoNext));

            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsPlayVisible = true;
                IsPauseVisible = false;
            });
        }

        private async Task GotoList()
        {
            if (_audioService.IsPlaying())
                _audioService.Stop();
            await Shell.Current.GoToAsync("///SouratesPage");

        }


        private async Task TapCommandHandler(int ayaNo)
        {
            System.Diagnostics.Debug.WriteLine(
    $"AYAH CLIQUÉE : {ayaNo}");
            try
            {
                var selectedAyah = SurahJsonData
                    .FirstOrDefault(a => a.AyaNo == ayaNo);

                if (selectedAyah == null)
                    return;

                // Position de début de l'ayah
                double start = selectedAyah.Start;

                // Si le début n'est pas renseigné,
                // utiliser la fin de l'ayah précédente.
                if (ayaNo > 1 && start <= 0)
                {
                    var previousAyah = SurahJsonData
                        .FirstOrDefault(a => a.AyaNo == ayaNo - 1);

                    if (previousAyah == null || previousAyah.End <= 0)
                        return;

                    start = previousAyah.End;
                }

                if (start < 0)
                    return;

                // Conserver l'ayah sélectionnée
                _currentAyah = selectedAyah;

                // Déplacer la lecture et reprendre
                await _audioService.ResumeFromAsync(start);

                // Surlignage
                HighlightAyah(ayaNo);

                // Scroll vers l'ayah exacte
                ScrollToSpanRequested?.Invoke(ayaNo);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsPlayVisible = false;
                    IsPauseVisible = true;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Erreur lecture ayah {ayaNo}: {ex}");
            }
        }





        private void OnAyahChanged(int index)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (SurahJsonData == null || SurahJsonData.Count == 0)
                    return;

                if (index > SurahJsonData.Count)
                {
                    IsPauseVisible = false;
                    IsPlayVisible = true;
                }

                var ayah = SurahJsonData.FirstOrDefault(a => a.AyaNo == index);

                if (ayah == null)
                    return;

                _currentAyah = ayah;

             

                // 🔊 IMPORTANT : ne recharge pas l'audio
                _audioService.PlayAyah(ayah);
                if(index == 1)
                { 
                    await Task.Delay(6000);

                }
                // coloration
                HighlightAyah(index);
                ScrollToSpanRequested?.Invoke(index - 1);
            });
        }

        #endregion // end Private Methods
    }
}
