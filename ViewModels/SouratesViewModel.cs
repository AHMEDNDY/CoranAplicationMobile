
using CoranWarshSynchroniser.Models;
using CoranWarshSynchroniser.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace CoranWarshSynchroniser.ViewModels
{
    public class SouratesViewModel : BaseViewModel
    {
        private readonly List<Sourate> _allSurahs = new();

        private Sourate? _selectedSourate;
        private string _searchText = string.Empty;
        private bool? _placeFilter;
        private bool _isLoading;
        private bool _isNavigating;

        private Color _medinaButtonColor =
            Color.FromArgb("#4A90E2");

        private Color _meccaButtonColor =
            Color.FromArgb("#4A90E2");

        // Une seule déclaration de Surahs
        public ObservableCollection<Sourate> Surahs { get; } = new();

        // Alias facultatif pour les anciens bindings
        public ObservableCollection<Sourate> Sourates => Surahs;

        public ICommand SurahTappedCommand { get; }
        public ICommand MeccaCommand { get; }
        public ICommand MedinaCommand { get; }
        public ICommand SelectSourateCommand { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(
                    ref _searchText,
                    value ?? string.Empty))
                {
                    FilterSurahs();
                }
            }
        }

        public Sourate? SelectedSourate
        {
            get => _selectedSourate;
            set
            {
                if (SetProperty(ref _selectedSourate, value)
                    && value != null)
                {
                    _ = NavigateToSurahAsync(value);
                }
            }
        }

        public Color MedinaButtonColor
        {
            get => _medinaButtonColor;
            set => SetProperty(ref _medinaButtonColor, value);
        }

        public Color MeccaButtonColor
        {
            get => _meccaButtonColor;
            set => SetProperty(ref _meccaButtonColor, value);
        }

        private static readonly HashSet<int> MedinaSurahs = new()
        {
            2, 3, 4, 5, 8, 9, 13, 22, 24, 33,
            47, 48, 49, 55, 57, 58, 59, 60,
            61, 62, 63, 64, 65, 66, 76, 98,
            99, 110
        };

        public SouratesViewModel()
        {
            SurahTappedCommand =
                new Command<Sourate>(
                    async surah => await NavigateToSurahAsync(surah));

            SelectSourateCommand = SurahTappedCommand;

            MeccaCommand =
                new Command(() => SetPlaceFilter(true));

            MedinaCommand =
                new Command(() => SetPlaceFilter(false));
        }

        // Compatibilité avec les appels existants
       

        // Chargement du fichier surahs.json
        public async Task LoadSurah()
        {
            if (IsLoading)
                return;

            try
            {
                IsLoading = true;

                using var stream =
                    await FileSystem.OpenAppPackageFileAsync(
                        "surahs.json");

                using var reader = new StreamReader(stream);

                var json = await reader.ReadToEndAsync();

                var ayahs =
                    JsonSerializer.Deserialize<List<AyahSource>>(json)
                    ?? new List<AyahSource>();

                // Regrouper les versets par sourate
                var sourates = ayahs
                    .Where(a => a.SuraNo >= 1 && a.SuraNo <= 114)
                    .GroupBy(a => a.SuraNo)
                    .OrderBy(g => g.Key)
                    .Select(g => new Sourate
                    {
                        Id = g.Key,

                        Name = g.First().SuraNameAr?.Trim()
                               ?? string.Empty,

                        TotalVerses = g
                            .Select(a => a.AyaNo)
                            .Distinct()
                            .Count(),

                        IsMecca =
                            !MedinaSurahs.Contains(g.Key)
                    })
                    .ToList();

                // Conserver toutes les sourates
                _allSurahs.Clear();
                _allSurahs.AddRange(sourates);

                // Actualiser la CollectionView
                await MainThread.InvokeOnMainThreadAsync(
                    FilterSurahs);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Erreur LoadSurah : {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Filtrer par lieu de révélation
        private void SetPlaceFilter(bool isMecca)
        {
            // Deuxième clic : désactiver le filtre
            _placeFilter = _placeFilter == isMecca
                ? null
                : isMecca;

            MeccaButtonColor = _placeFilter == true
                ? Colors.Red
                : Color.FromArgb("#4A90E2");

            MedinaButtonColor = _placeFilter == false
                ? Colors.Red
                : Color.FromArgb("#4A90E2");

            FilterSurahs();
        }

        // Recherche + filtre Mecca/Medina
        private void FilterSurahs()
        {
            IEnumerable<Sourate> result = _allSurahs;

            // Filtrer les sourates
            if (_placeFilter.HasValue)
            {
                result = result.Where(s =>
                    s.IsMecca == _placeFilter.Value);
            }

            // Rechercher par nom ou numéro
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = NormalizeArabic(
                    SearchText.Trim());

                result = result.Where(s =>
                    NormalizeArabic(s.Name).Contains(search)
                    || s.Id.ToString().Contains(SearchText.Trim()));
            }

            Surahs.Clear();

            foreach (var item in result)
            {
                Surahs.Add(item);
            }
        }

        // Supprimer les accents arabes pour la recherche
        private static string NormalizeArabic(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string normalized =
                text.Normalize(NormalizationForm.FormD);

            var builder = new StringBuilder();

            foreach (char c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);

                if (category == UnicodeCategory.NonSpacingMark
                    || category == UnicodeCategory.SpacingCombiningMark
                    || category == UnicodeCategory.EnclosingMark)
                    continue;

                char character = c switch
                {
                    'أ' or 'إ' or 'آ' or 'ٱ' => 'ا',
                    'ى' => 'ي',
                    'ؤ' => 'و',
                    'ئ' => 'ي',
                    _ => c
                };

                builder.Append(character);
            }

            return builder.ToString()
                .Normalize(NormalizationForm.FormC);
        }

        // Navigation vers QuranReaderPage
        private async Task NavigateToSurahAsync(Sourate? surah)
        {
            if (surah == null || _isNavigating)
                return;

            _isNavigating = true;

            try
            {
                await Shell.Current.GoToAsync(
                    nameof(Views.QuranReaderPage),
                    new Dictionary<string, object>
                    {
                        { "SourateId", surah.Id }
                    });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Erreur navigation : {ex.Message}");
            }
            finally
            {
                _selectedSourate = null;
                OnPropertyChanged(nameof(SelectedSourate));

                _isNavigating = false;
            }
        }
    }
}
