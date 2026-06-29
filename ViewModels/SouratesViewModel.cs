using CoranWarshSynchroniser.Models;
using CoranWarshSynchroniser.Services;
using MvvmHelpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CoranWarshSynchroniser.ViewModels
{
   public class SouratesViewModel : BaseViewModel
    {
        private Sourate _selectedSourate;
        private string _searchText = string.Empty;
        private readonly SurahService _service;
        private ObservableCollection<Sourate> _surahs;

        public ICommand MeccaCommand { get; }
        public ICommand MedinaCommand { get; }

        //public ObservableCollection<Sourate> Surahs { get; } = new();

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterSurahs();
            }
        }
        public Sourate SelectedSourate
        {
            get => _selectedSourate;
            set
            {
                if (value == null) return; // ignorer le reset

                _selectedSourate = value;
                OnPropertyChanged();

                // Fire-and-forget propre
                _ = OnSourateSelectedAsync(value);
            }
        }
        private bool _isNavigating = false;

        private async Task OnSourateSelectedAsync(Sourate sourate)
        {
            if (sourate == null || _isNavigating) return;

            _isNavigating = true;

            try
            {
                await Shell.Current.GoToAsync($"QuranReaderPage?SourateId={sourate.Id}");
            }
            finally
            {
                // Reset APRÈS la navigation, sur le thread UI
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    _selectedSourate = null;
                    OnPropertyChanged(nameof(SelectedSourate));
                });

                _isNavigating = false;
            }
        }
        public ICommand SurahTappedCommand { get; }
        //public ICommand ActionCommand { get; }
        public ObservableCollection<Sourate> Sourates { get; set; }
        //public ObservableCollection<Sourate> FilteredSourates
        //{
        //    get => _filteredSourates;
        //    set
        //    {
        //        _filteredSourates = value;
        //        OnPropertyChanged();
        ////    }
        //}
        private Color _medinaButtonColor = Color.FromArgb("#4A90E2");
        public Color MedinaButtonColor
        {
            get => _medinaButtonColor;
            set
            {
                _medinaButtonColor = value;
                OnPropertyChanged();
            }
        }

        private Color _meccaButtonColor = Color.FromArgb("#4A90E2");
        public Color MeccaButtonColor
        {
            get => _meccaButtonColor;
            set
            {
                _meccaButtonColor = value;
                OnPropertyChanged();
            }
        }

        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public SouratesViewModel() : this(new SurahService())
        {
        }

       
        public SouratesViewModel(SurahService service)
        {
            _service = service;
            _surahs = new ObservableCollection<Sourate>(_service.GetAll());
            SurahTappedCommand = new Command<Sourate>(OnSurahTapped);
            MedinaCommand = new Command(() =>
            {
                MedinaButtonColor = Colors.Red;
                MeccaButtonColor = Color.FromArgb("#4A90E2");

                GotoList(false);
            });

            MeccaCommand = new Command(() =>
            {
                MeccaButtonColor = Colors.Red;
                MedinaButtonColor = Color.FromArgb("#4A90E2");

                GotoList(true);
            });
        }

        private void GotoList(bool mecca)
        {
            var list = _service.Mecca(mecca);

            Surahs.Clear();

            foreach (var item in list)
                Surahs.Add(item);
        }
        public ICommand SelectSourateCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        

        public ObservableCollection<Sourate> Surahs
        {
            get => _surahs;
            set { _surahs = value; 
                OnPropertyChanged();
            }
        }


        #region methodes privés
        private async void OnSurahTapped(Sourate surah)
        {
            if (surah is null)
                return;

            await Shell.Current.GoToAsync(
                nameof(Views.QuranReaderPage),
                new Dictionary<string, object>
                {
                    { "SourateId", surah.Id }
                });
        }
        private void FilterSurahs()
        {
            MeccaButtonColor = Color.FromArgb("#4A90E2");
            MedinaButtonColor = Color.FromArgb("#4A90E2");
            var results = _service.Search(_searchText);
            Surahs.Clear();

            foreach (var item in results)
                Surahs.Add(item);
        }
        
        #endregion

    }
}
