using CoranWarshSynchroniser.Models;
using CoranWarshSynchroniser.Services;
using MvvmHelpers;
using Newtonsoft.Json;
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

        public SouratesViewModel() : this(new SurahService())
        {
        }

       
        public SouratesViewModel(SurahService service)
        {
            

            _service = service;
            _surahs = new ObservableCollection<Sourate>(_service.GetAll());
            SurahTappedCommand = new Command<Sourate>(OnSurahTapped);
        }

       
        public ICommand SelectSourateCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            var results = _service.Search(_searchText);
            Surahs = new ObservableCollection<Sourate>(results);
        }
        
        #endregion

    }
}
