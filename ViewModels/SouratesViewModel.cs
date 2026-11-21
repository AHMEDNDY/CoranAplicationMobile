using CoranWarshSynchroniser.Models;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace CoranWarshSynchroniser.ViewModels;

// Un objet d'affichage : permet de mettre à jour l'étoile sans modifier Models/Sourate.cs.
public class SourateDisplayItem : BaseViewModel
{
    private bool _isFavorite;
    public Sourate Sourate { get; }

    public int Id => Sourate.Id;
    public bool IsMecca => Sourate.IsMecca;
    public string DisplayName => Sourate.Name.TrimStart().StartsWith("", StringComparison.Ordinal)
        ? Sourate.Name
        : $" {Sourate.Name}";
    public string LatinName { get; }
    public string VerseLabel => $"{Sourate.TotalVerses} آيات";

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

    public string FavoriteSymbol => IsFavorite ? "★" : "☆";
    public Color FavoriteColor => IsFavorite
        ? Color.FromArgb("#DCA332")
        : Color.FromArgb("#102C5C");

    public SourateDisplayItem(Sourate sourate, bool favorite, string latinName)
    {
        Sourate = sourate;
        _isFavorite = favorite;
        LatinName = latinName;
    }
}

public class SouratesViewModel : BaseViewModel
{
    private enum SurahFilter { All, Mecca, Medina, Favorites }

    private readonly List<Sourate> _allSurahs = new();
    private readonly HashSet<int> _favoriteIds = new();
    private SurahFilter _activeFilter = SurahFilter.All;
    private Sourate? _selectedSourate;
    private string _searchText = string.Empty;
    private bool _isLoading;
    private bool _isNavigating;
    private int _resultsCount;
    private const string FavoritesPreferencesKey = "quran_warsh_favorite_surahs";

    private static readonly Color ActiveBlue = Color.FromArgb("#073A86");
    private static readonly Color Gold = Color.FromArgb("#E2AD34");
    private static readonly Color Ink = Color.FromArgb("#112A55");

    // Conservées pour compatibilité avec tes autres fichiers.
    public ObservableCollection<Sourate> Surahs { get; } = new();
    public ObservableCollection<Sourate> Sourates => Surahs;

    // La nouvelle page utilise les lignes enrichies pour les favoris et la translittération.
    public ObservableCollection<SourateDisplayItem> DisplaySurahs { get; } = new();

    public ICommand SurahTappedCommand { get; }
    public ICommand SelectSourateCommand { get; }
    public ICommand AllCommand { get; }
    public ICommand MeccaCommand { get; }
    public ICommand MedinaCommand { get; }
    public ICommand FavoritesCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand BackCommand { get; }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public int ResultsCount
    {
        get => _resultsCount;
        private set => SetProperty(ref _resultsCount, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value ?? string.Empty))
                FilterSurahs();
        }
    }

    public Sourate? SelectedSourate
    {
        get => _selectedSourate;
        set
        {
            if (SetProperty(ref _selectedSourate, value) && value != null)
                _ = NavigateToSurahAsync(value);
        }
    }

    // Propriétés de couleur de tes 4 filtres (la vue les met à jour automatiquement).
    private Color FilterBackground(SurahFilter filter) =>
        _activeFilter == filter ? ActiveBlue : Colors.White;
    private Color FilterText(SurahFilter filter) =>
        _activeFilter == filter ? Colors.White : Ink;
    private Color FilterBorder(SurahFilter filter) =>
        _activeFilter == filter ? Gold : Color.FromArgb("#CFDAEB");

    public Color AllButtonColor => FilterBackground(SurahFilter.All);
    public Color MeccaButtonColor => FilterBackground(SurahFilter.Mecca);
    public Color MedinaButtonColor => FilterBackground(SurahFilter.Medina);
    public Color FavoritesButtonColor => FilterBackground(SurahFilter.Favorites);

    public Color AllTextColor => FilterText(SurahFilter.All);
    public Color MeccaTextColor => FilterText(SurahFilter.Mecca);
    public Color MedinaTextColor => FilterText(SurahFilter.Medina);
    public Color FavoritesTextColor => FilterText(SurahFilter.Favorites);

    public Color AllBorderColor => FilterBorder(SurahFilter.All);
    public Color MeccaBorderColor => FilterBorder(SurahFilter.Mecca);
    public Color MedinaBorderColor => FilterBorder(SurahFilter.Medina);
    public Color FavoritesBorderColor => FilterBorder(SurahFilter.Favorites);

    private static readonly HashSet<int> MedinaSurahs = new()
    {
        2, 3, 4, 5, 8, 9, 13, 22, 24, 33,
        47, 48, 49, 55, 57, 58, 59, 60,
        61, 62, 63, 64, 65, 66, 76, 98,
        99, 110
    };

    // Translittérations : index 0 = sourate n°1.
    private static readonly string[] LatinNames =
    {
        "Al-Fātiḥah", "Al-Baqarah", "Āl ‘Imrān", "An-Nisā’", "Al-Mā’idah",
        "Al-An‘ām", "Al-A‘rāf", "Al-Anfāl", "At-Tawbah", "Yūnus",
        "Hūd", "Yūsuf", "Ar-Ra‘d", "Ibrāhīm", "Al-Ḥijr",
        "An-Naḥl", "Al-Isrā’", "Al-Kahf", "Maryam", "Ṭā Hā",
        "Al-Anbiyā’", "Al-Ḥajj", "Al-Mu’minūn", "An-Nūr", "Al-Furqān",
        "Ash-Shu‘arā’", "An-Naml", "Al-Qaṣaṣ", "Al-‘Ankabūt", "Ar-Rūm",
        "Luqmān", "As-Sajdah", "Al-Aḥzāb", "Saba’", "Fāṭir",
        "Yā Sīn", "Aṣ-Ṣāffāt", "Ṣād", "Az-Zumar", "Ghāfir",
        "Fuṣṣilat", "Ash-Shūrā", "Az-Zukhruf", "Ad-Dukhān", "Al-Jāthiyah",
        "Al-Aḥqāf", "Muḥammad", "Al-Fatḥ", "Al-Ḥujurāt", "Qāf",
        "Adh-Dhāriyāt", "Aṭ-Ṭūr", "An-Najm", "Al-Qamar", "Ar-Raḥmān",
        "Al-Wāqi‘ah", "Al-Ḥadīd", "Al-Mujādilah", "Al-Ḥashr", "Al-Mumtaḥanah",
        "Aṣ-Ṣaff", "Al-Jumu‘ah", "Al-Munāfiqūn", "At-Taghābun", "Aṭ-Ṭalāq",
        "At-Taḥrīm", "Al-Mulk", "Al-Qalam", "Al-Ḥāqqah", "Al-Ma‘ārij",
        "Nūḥ", "Al-Jinn", "Al-Muzzammil", "Al-Muddaththir", "Al-Qiyāmah",
        "Al-Insān", "Al-Mursalāt", "An-Naba’", "An-Nāzi‘āt", "‘Abasa",
        "At-Takwīr", "Al-Infiṭār", "Al-Muṭaffifīn", "Al-Inshiqāq", "Al-Burūj",
        "Aṭ-Ṭāriq", "Al-A‘lā", "Al-Ghāshiyah", "Al-Fajr", "Al-Balad",
        "Ash-Shams", "Al-Layl", "Aḍ-Ḍuḥā", "Ash-Sharḥ", "At-Tīn",
        "Al-‘Alaq", "Al-Qadr", "Al-Bayyinah", "Az-Zalzalah", "Al-‘Ādiyāt",
        "Al-Qāri‘ah", "At-Takāthur", "Al-‘Aṣr", "Al-Humazah", "Al-Fīl",
        "Quraysh", "Al-Mā‘ūn", "Al-Kawthar", "Al-Kāfirūn", "An-Naṣr",
        "Al-Masad", "Al-Ikhlāṣ", "Al-Falaq", "An-Nās"
    };

    private static string LatinNameFor(int surahId) =>
        surahId >= 1 && surahId <= LatinNames.Length ? LatinNames[surahId - 1] : "";

    public SouratesViewModel()
    {
        SurahTappedCommand = new Command<Sourate>(async surah => await NavigateToSurahAsync(surah));
        SelectSourateCommand = SurahTappedCommand;

        AllCommand = new Command(() => SetFilter(SurahFilter.All));
        MeccaCommand = new Command(() => SetFilter(SurahFilter.Mecca));
        MedinaCommand = new Command(() => SetFilter(SurahFilter.Medina));
        FavoritesCommand = new Command(() => SetFilter(SurahFilter.Favorites));
        ToggleFavoriteCommand = new Command<SourateDisplayItem>(ToggleFavorite);
        BackCommand = new Command(async () => await GoBackAsync());

        ReadFavoriteIds();
    }

    public async Task LoadSurah()
    {
        if (IsLoading || _allSurahs.Count > 0) return;

        try
        {
            IsLoading = true;
            using var stream = await FileSystem.OpenAppPackageFileAsync("surahs.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            var ayahs = JsonSerializer.Deserialize<List<AyahSource>>(json) ?? new List<AyahSource>();

            var sourates = ayahs
                .Where(a => a.SuraNo >= 1 && a.SuraNo <= 114)
                .GroupBy(a => a.SuraNo)
                .OrderBy(group => group.Key)
                .Select(group => new Sourate
                {
                    Id = group.Key,
                    Name = group.First().SuraNameAr?.Trim() ?? string.Empty,
                    TotalVerses = group.Select(a => a.AyaNo).Distinct().Count(),
                    IsMecca = !MedinaSurahs.Contains(group.Key)
                })
                .ToList();

            _allSurahs.Clear();
            _allSurahs.AddRange(sourates);
            await MainThread.InvokeOnMainThreadAsync(FilterSurahs);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur LoadSurah : {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SetFilter(SurahFilter filter)
    {
        if (_activeFilter == filter) return;
        _activeFilter = filter;
        foreach (var property in new[]
        {
            nameof(AllButtonColor), nameof(MeccaButtonColor),
            nameof(MedinaButtonColor), nameof(FavoritesButtonColor),
            nameof(AllTextColor), nameof(MeccaTextColor),
            nameof(MedinaTextColor), nameof(FavoritesTextColor),
            nameof(AllBorderColor), nameof(MeccaBorderColor),
            nameof(MedinaBorderColor), nameof(FavoritesBorderColor)
        })
        {
            OnPropertyChanged(property);
        }
        FilterSurahs();
    }

    private void FilterSurahs()
    {
        IEnumerable<Sourate> result = _allSurahs;
        result = _activeFilter switch
        {
            SurahFilter.Mecca => result.Where(s => s.IsMecca),
            SurahFilter.Medina => result.Where(s => !s.IsMecca),
            SurahFilter.Favorites => result.Where(s => _favoriteIds.Contains(s.Id)),
            _ => result
        };

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var query = NormalizeArabic(SearchText.Trim());
            result = result.Where(s =>
                NormalizeArabic(s.Name).Contains(query, StringComparison.OrdinalIgnoreCase)
                || NormalizeArabic($"سورة {s.Name}").Contains(query, StringComparison.OrdinalIgnoreCase)
                || s.Id.ToString(CultureInfo.InvariantCulture).Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase)
                || LatinNameFor(s.Id).Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        // Les anciens bindings sont conservés, en plus des nouveaux.
        var filtered = result.ToList();
        Surahs.Clear();
        DisplaySurahs.Clear();

        foreach (var surah in filtered)
        {
            Surahs.Add(surah);
            DisplaySurahs.Add(new SourateDisplayItem(
                surah, _favoriteIds.Contains(surah.Id), LatinNameFor(surah.Id)));
        }
        ResultsCount = filtered.Count;
    }

    private void ToggleFavorite(SourateDisplayItem? item)
    {
        if (item is null) return;
        if (!_favoriteIds.Add(item.Id)) _favoriteIds.Remove(item.Id);

        item.IsFavorite = _favoriteIds.Contains(item.Id);
        Preferences.Default.Set(FavoritesPreferencesKey,
            string.Join(",", _favoriteIds.OrderBy(id => id)));

        if (_activeFilter == SurahFilter.Favorites)
            FilterSurahs(); // La ligne disparaît lorsque retirée des favoris.
    }

    private void ReadFavoriteIds()
    {
        var saved = Preferences.Default.Get(FavoritesPreferencesKey, string.Empty);
        foreach (string chunk in saved.Split(',', StringSplitOptions.RemoveEmptyEntries))
            if (int.TryParse(chunk, out int id) && id is >= 1 and <= 114)
                _favoriteIds.Add(id);
    }

    private static string NormalizeArabic(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        var normalized = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (char character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category == UnicodeCategory.NonSpacingMark
                || category == UnicodeCategory.SpacingCombiningMark
                || category == UnicodeCategory.EnclosingMark
                || character == 'ـ') continue;

            builder.Append(character switch
            {
                'أ' or 'إ' or 'آ' or 'ٱ' => 'ا',
                'ى' => 'ي',
                'ؤ' => 'و',
                'ئ' => 'ي',
                _ => character
            });
        }
        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    private async Task GoBackAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Retour impossible : {ex.Message}");
        }
    }

    private async Task NavigateToSurahAsync(Sourate? surah)
    {
        if (surah is null || _isNavigating) return;
        _isNavigating = true;
        try
        {
            await Shell.Current.GoToAsync(
                nameof(Views.QuranReaderPage),
                new Dictionary<string, object> { { "SourateId", surah.Id } });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur navigation : {ex}");
        }
        finally
        {
            _selectedSourate = null;
            OnPropertyChanged(nameof(SelectedSourate));
            _isNavigating = false;
        }
    }
}
