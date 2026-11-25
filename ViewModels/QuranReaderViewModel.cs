using CoranWarshSynchroniser.Models;
using CoranWarshSynchroniser.Services;
using MvvmHelpers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Input;

namespace CoranWarshSynchroniser.ViewModels;

public class QuranReaderViewModel : BaseViewModel
{
    private readonly AudioService _audioService;
    private readonly IDispatcherTimer _syncTimer;
    private const string FavoriteKey = "quran_warsh_favorite_surahs";
    private int _loadedAudioSurah = -1;
    private bool _hasStartedAudio;
    private bool _isSeeking;
    private Surah? _currentAyah;

    private ObservableCollection<Surah> _surahJsonData = new();
    private string _name = string.Empty;
    private string _nameEnglish = string.Empty;
    private int _currentSurahNumber;
    private int _totalverse;
    private bool _isMecca;
    private bool _isLoading;
    private bool _isFavorite;
    private bool _showBasmalaHeader;
    private bool _isPlayVisible = true;
    private bool _isPauseVisible;
    private double _audioPositionSeconds;
    private double _audioDurationSeconds = 1;

    public Action<int>? ScrollToSpanRequested { get; set; }

    public ObservableCollection<Surah> SurahJsonData
    {
        get => _surahJsonData;
        set => SetProperty(ref _surahJsonData, value);
    }

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value ?? string.Empty))
                OnPropertyChanged(nameof(SurahTitle));
        }
    }

    public string NameEnglish
    {
        get => _nameEnglish;
        set => SetProperty(ref _nameEnglish, value ?? string.Empty);
    }

    public string SurahTitle
    {
        get
        {
            var label = Name.Trim();
            if (label.Length == 0) return "القرآن الكريم";
            // Évite "سورة سورة ...", y compris si le mot est vocalisé : سُورَةُ.
            string withoutMarks = new string(label.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark
                          && CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.SpacingCombiningMark)
                .ToArray());
            return withoutMarks.StartsWith("سورة", StringComparison.Ordinal)
                ? label : "سورة " + label;
        }
    }

    public bool IsMecca
    {
        get => _isMecca;
        set
        {
            if (SetProperty(ref _isMecca, value))
                OnPropertyChanged(nameof(RevelationPlace));
        }
    }
    public string RevelationPlace => IsMecca ? "مكية" : "مدنية";

    public int Totalverse
    {
        get => _totalverse;
        set
        {
            if (SetProperty(ref _totalverse, value))
                OnPropertyChanged(nameof(VerseCountText));
        }
    }
    public string VerseCountText => $"عدد آياتها {Totalverse}";

    public int CurrentSurahNumber
    {
        get => _currentSurahNumber;
        set
        {
            if (SetProperty(ref _currentSurahNumber, value))
                OnPropertyChanged(nameof(CanGoNext));
        }
    }
    public bool CanGoNext => CurrentSurahNumber < 114;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool ShowBasmalaHeader
    {
        get => _showBasmalaHeader;
        set => SetProperty(ref _showBasmalaHeader, value);
    }

    public bool IsPlayVisible
    {
        get => _isPlayVisible;
        set => SetProperty(ref _isPlayVisible, value);
    }
    public bool IsPauseVisible
    {
        get => _isPauseVisible;
        set => SetProperty(ref _isPauseVisible, value);
    }

    public double AudioPositionSeconds
    {
        get => _audioPositionSeconds;
        set
        {
            if (SetProperty(ref _audioPositionSeconds, value))
                OnPropertyChanged(nameof(CurrentTimeText));
        }
    }
    public double AudioDurationSeconds
    {
        get => _audioDurationSeconds;
        set
        {
            if (SetProperty(ref _audioDurationSeconds, Math.Max(1, value)))
                OnPropertyChanged(nameof(DurationTimeText));
        }
    }

    public string CurrentTimeText => FormatTime(AudioPositionSeconds);
    public string DurationTimeText => FormatTime(AudioDurationSeconds);
    private static string FormatTime(double seconds)
    {
        var time = TimeSpan.FromSeconds(Math.Max(0, double.IsFinite(seconds) ? seconds : 0));
        return time.TotalHours >= 1 ? time.ToString(@"h\:mm\:ss") : time.ToString(@"m\:ss");
    }

    public bool IsFavorite
    {
        get => _isFavorite;
        set
        {
            if (SetProperty(ref _isFavorite, value))
            {
                OnPropertyChanged(nameof(FavoriteSymbol));
                OnPropertyChanged(nameof(FavoriteColor));
            }
        }
    }
    public string FavoriteSymbol => IsFavorite ? "♥" : "♡";
    public Color FavoriteColor => IsFavorite ? Color.FromArgb("#F7C75E") : Color.FromArgb("#D59D36");

    public ICommand PlayCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand NextSurahCommand { get; }
    public ICommand PreviousSurahCommand { get; }
    public ICommand GotoPageListeCommand { get; }
    public ICommand TapCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand RepeatSurahCommand { get; }

    public QuranReaderViewModel()
    {
        _audioService = new AudioService();
        PlayCommand = new Command(async () => await PlayCommandHandler());
        PauseCommand = new Command(PauseCommandHandler);
        NextSurahCommand = new Command(async () => await NextSurahCommandHandler());
        PreviousSurahCommand = new Command(async () => await PreviousSurahCommandHandler());
        GotoPageListeCommand = new Command(async () => await GotoList());
        TapCommand = new Command<int>(async ayaNo => await TapCommandHandler(ayaNo));
        ToggleFavoriteCommand = new Command(ToggleFavorite);
        RepeatSurahCommand = new Command(async () => await RepeatSurahAsync());

        // Un seul abonnement, et plus de PlayAyah lors du changement automatique de verset.
        _audioService.AyahChanged += OnAyahChanged;

        _syncTimer = Application.Current!.Dispatcher.CreateTimer();
        _syncTimer.Interval = TimeSpan.FromMilliseconds(250);
        _syncTimer.Tick += SyncTick;
        _syncTimer.Start();

        // NE PAS appeler LoadSurah() dans le constructeur : Shell n'a pas
        // encore transmis le paramètre SourateId à la page.
    }

    // Conservé si d'autres pages appellent encore cette méthode.
    public void LoadSourate(int id) => _ = LoadSourateAsync(id);

    public async Task LoadSourateAsync(int id)
    {
        if (id < 1 || id > 114) return;
        try
        {
            _audioService.Stop();
            _loadedAudioSurah = -1;
            _hasStartedAudio = false;
            CurrentSurahNumber = id;
            IsPlayVisible = true;
            IsPauseVisible = false;
            AudioPositionSeconds = 0;
            await LoadSurah();
            RefreshFavorite();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadSourateAsync({id}) : {ex}");
        }
    }

    public async Task LoadSurah()
    {
        if (CurrentSurahNumber < 1 || CurrentSurahNumber > 114) return;
        IsLoading = true;
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("surahs.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            var all = JsonConvert.DeserializeObject<List<Surah>>(json) ?? new List<Surah>();
            var items = all.Where(a => a.SuraNo == CurrentSurahNumber)
                           .OrderBy(a => a.AyaNo).ToList();

            SurahJsonData = new ObservableCollection<Surah>(items);
            _currentAyah = items.FirstOrDefault();
            if (_currentAyah == null)
            {
                Name = "";
                NameEnglish = "";
                Totalverse = 0;
                ShowBasmalaHeader = false;
                AudioDurationSeconds = 1;
                return;
            }

            Name = _currentAyah.SuraNameAr ?? "";
            NameEnglish = _currentAyah.SuraNameEn ?? "";
            Totalverse = _currentAyah.TotalVerses > 0
                ? _currentAyah.TotalVerses : items.Count;
            IsMecca = _currentAyah.IsMecca;

            // La basmala ne doit pas être affichée deux fois si le JSON la compte
            // dans le texte du premier verset ; aucune numérotation n'est modifiée.
            ShowBasmalaHeader = CurrentSurahNumber != 9
                && !ContainsBasmala(_currentAyah.AyaText ?? "");

            foreach (var ayah in items)
            {
                ayah.ShowTranslation = false; // visuel fidèle ; remettre true si voulu
                ayah.Translation ??= "";
                ayah.TextColor = Color.FromArgb("#0B244C");
                ayah.AyahBackgroundColor = Color.FromArgb("#FFFDF8");
                ayah.TranslationColor = Color.FromArgb("#6A7484");
            }

            // Durée issue des timings JSON ; ne prétend pas mesurer le MP3.
            AudioDurationSeconds = Math.Max(1, items.Max(a => Math.Max(a.End, a.Start)));
            AudioPositionSeconds = 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadSurah : {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static bool ContainsBasmala(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var chars = new StringBuilder();
        foreach (var ch in normalized)
        {
            var c = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (c == UnicodeCategory.NonSpacingMark ||
                c == UnicodeCategory.SpacingCombiningMark ||
                c == UnicodeCategory.EnclosingMark)
                continue;
            if (ch == 'ـ') continue;
            chars.Append(ch == 'ٱ' ? 'ا' : ch);
        }
        return chars.ToString().Contains("بسم الله", StringComparison.Ordinal);
    }

    // API publique conservée pour compatibilité.
    public async Task LoadSurahAsync(int surahNumber)
    {
        if (surahNumber == _loadedAudioSurah)
            return;

        string audioFile = $"Audio/{surahNumber:D3}.mp3";

        await _audioService.LoadSurahAudioAsync(audioFile);

        _loadedAudioSurah = surahNumber;

        _hasStartedAudio = false;

        double duration = _audioService.GetDurationSeconds();

        if (duration > 0)
        {
            AudioDurationSeconds = duration;
        }
    }


    private void SyncTick(object? sender, EventArgs e)
    {
        if (!_hasStartedAudio || _isSeeking)
            return;

        try
        {
            double current = _audioService
                .GetCurrentPosition()
                .TotalSeconds;

            AudioPositionSeconds = Math.Clamp(
                current,
                0,
                AudioDurationSeconds);

            if (!_audioService.IsPlaying())
            {
                IsPlayVisible = true;
                IsPauseVisible = false;

                return;
            }

            var ayah = SurahJsonData
                .FirstOrDefault(a =>
                    a.Start <= current &&
                    a.End > current);

            if (ayah != null &&
                _currentAyah?.AyaNo != ayah.AyaNo)
            {
                _currentAyah = ayah;

                HighlightAyah(ayah.AyaNo);

                ScrollToSpanRequested?.Invoke(ayah.AyaNo);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"SyncTick : {ex.Message}");
        }
    }

  
    public void StopPlayer()
    {
        _syncTimer.Stop();

        _audioService.Stop();

        _loadedAudioSurah = -1;
        _hasStartedAudio = false;
        _isSeeking = false;

        IsPlayVisible = true;
        IsPauseVisible = false;
    }

    public void StartSynchronization()
    {
        if (!_syncTimer.IsRunning)
        {
            _syncTimer.Start();
        }
    }

    private void HighlightAyah(int ayaNo)
    {
        foreach (var ayah in SurahJsonData)
        {
            ayah.TextColor = Color.FromArgb("#0B244C");
            ayah.AyahBackgroundColor = Color.FromArgb("#FFFDF8");
        }
        var selected = SurahJsonData.FirstOrDefault(a => a.AyaNo == ayaNo);
        if (selected != null)
        {
            selected.TextColor = Color.FromArgb("#063A62");
            selected.AyahBackgroundColor = Color.FromArgb("#BDEEFF");
        }
    }

    private async Task PlayCommandHandler()
    {
        try
        {
            if (CurrentSurahNumber < 1) return;
            if (SurahJsonData.Count == 0) await LoadSurah();
            if (SurahJsonData.Count == 0) return;
            if (_audioService.IsPlaying()) return;

            await LoadSurahAsync(CurrentSurahNumber);
            _currentAyah ??= SurahJsonData.First();

            if (_hasStartedAudio)
                await _audioService.ResumeFromAsync(AudioPositionSeconds);
            else
            {
                _audioService.PlayAyah(_currentAyah);
                _hasStartedAudio = true;
            }

            IsPlayVisible = false;
            IsPauseVisible = true;
            HighlightAyah(_currentAyah.AyaNo);
            ScrollToSpanRequested?.Invoke(_currentAyah.AyaNo);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PlayCommandHandler : {ex}");
            IsPlayVisible = true;
            IsPauseVisible = false;
        }
    }

    private void PauseCommandHandler()
    {
        if (!_audioService.IsPlaying()) return;
        _audioService.Pause();
        IsPlayVisible = true;
        IsPauseVisible = false;
    }

    private async Task NextSurahCommandHandler()
    {
        if (CurrentSurahNumber < 114)
            await LoadSourateAsync(CurrentSurahNumber + 1);
    }

    private async Task PreviousSurahCommandHandler()
    {
        if (CurrentSurahNumber > 1)
            await LoadSourateAsync(CurrentSurahNumber - 1);
    }

    private async Task GotoList()
    {
        try
        {
            _audioService.Stop();
            // La page lecteur est ouverte depuis la liste avec GoToAsync(QuranReaderPage).
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GotoList : {ex}");
        }
    }

    private async Task TapCommandHandler(int ayaNo)
    {
        try
        {
            var ayah = SurahJsonData.FirstOrDefault(a => a.AyaNo == ayaNo);
            if (ayah == null) return;

            double start = ayah.Start;
            if (start <= 0 && ayaNo > 1)
            {
                var previous = SurahJsonData.FirstOrDefault(a => a.AyaNo == ayaNo - 1);
                if (previous == null || previous.End <= 0) return;
                start = previous.End;
            }
            if (start < 0) return;

            await LoadSurahAsync(CurrentSurahNumber);
            _currentAyah = ayah;
            await _audioService.ResumeFromAsync(start);
            _hasStartedAudio = true;
            AudioPositionSeconds = start;
            IsPlayVisible = false;
            IsPauseVisible = true;
            HighlightAyah(ayaNo);
            ScrollToSpanRequested?.Invoke(ayaNo);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TapCommandHandler({ayaNo}) : {ex}");
        }
    }

    private void OnAyahChanged(int ayaNo)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var ayah = SurahJsonData.FirstOrDefault(a => a.AyaNo == ayaNo);
            if (ayah == null || _currentAyah?.AyaNo == ayaNo) return;
            _currentAyah = ayah;
            HighlightAyah(ayaNo);
            ScrollToSpanRequested?.Invoke(ayaNo);
        });
    }

    public void BeginSeeking()
    {
        _isSeeking = true;
    }

    public void EndSeeking()
    {
        _isSeeking = false;
    }

    public async Task SeekAsync(double positionSeconds)
    {
        if (CurrentSurahNumber < 1 ||
            SurahJsonData.Count == 0)
            return;

        _isSeeking = true;

        try
        {
            await LoadSurahAsync(CurrentSurahNumber);

            double position = Math.Clamp(
                positionSeconds,
                0,
                AudioDurationSeconds);

            // Reprendre la lecture à la position choisie
            await _audioService.ResumeFromAsync(position);

            _hasStartedAudio = true;

            AudioPositionSeconds = position;

            IsPlayVisible = false;
            IsPauseVisible = true;

            // Rechercher l'ayat correspondant à la position
            var ayah = SurahJsonData
                .FirstOrDefault(a =>
                    a.Start <= position &&
                    a.End > position);

            // Solution alternative
            if (ayah == null)
            {
                ayah = SurahJsonData
                    .Where(a => a.Start <= position)
                    .OrderByDescending(a => a.Start)
                    .FirstOrDefault();
            }

            if (ayah != null)
            {
                _currentAyah = ayah;

                HighlightAyah(ayah.AyaNo);

                ScrollToSpanRequested?.Invoke(ayah.AyaNo);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"SeekAsync : {ex}");
        }
        finally
        {
            _isSeeking = false;
        }
    }

    private async Task RepeatSurahAsync()
    {
        if (SurahJsonData.Count == 0) return;
        await SeekAsync(Math.Max(0, SurahJsonData[0].Start));
    }

    private void RefreshFavorite()
    {
        var ids = GetFavoriteIds();
        IsFavorite = ids.Contains(CurrentSurahNumber);
    }

    private static HashSet<int> GetFavoriteIds()
    {
        string raw = Preferences.Get(FavoriteKey, string.Empty);
        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(s => int.TryParse(s, out var n) ? n : 0)
                  .Where(n => n > 0)
                  .ToHashSet();
    }

    private void ToggleFavorite()
    {
        if (CurrentSurahNumber < 1) return;
        var ids = GetFavoriteIds();
        if (!ids.Add(CurrentSurahNumber)) ids.Remove(CurrentSurahNumber);
        Preferences.Set(FavoriteKey, string.Join(",", ids.OrderBy(n => n)));
        IsFavorite = ids.Contains(CurrentSurahNumber);
    }

    
}
