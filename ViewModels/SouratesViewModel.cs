using CoranWarshSynchroniser.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CoranWarshSynchroniser.ViewModels
{
   public class SouratesViewModel : INotifyPropertyChanged
    {
        private  ObservableCollection<Sourate> _allSourates;
        private ObservableCollection<Sourate> _filteredSourates;
        private Sourate _selectedSourate;

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


        public ICommand SearchCommand { get; }
        //public ICommand ActionCommand { get; }
        public ObservableCollection<Sourate> Sourates { get; set; }
        public ObservableCollection<Sourate> FilteredSourates
        {
            get => _filteredSourates;
            set
            {
                _filteredSourates = value;
                OnPropertyChanged();
            }
        }

        
        public SouratesViewModel()
        {
            GetAllSourates();
            _filteredSourates = new ObservableCollection<Sourate>(_allSourates);
            SearchCommand = new Command<string>(OnSearch);

            // ✅ Ajouter cette ligne
            SelectSourateCommand = new Command<Sourate>(async (s) => await OnSourateSelectedAsync(s));
        }

        public ICommand SelectSourateCommand { get; }


       
        private void OnSearch(string searchText) 
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilteredSourates = new ObservableCollection<Sourate>(_allSourates);
                return;
            }

            var filtered = _allSourates.Where(s =>
                s.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                s.Id.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase)

            ).ToList();

            FilteredSourates = new ObservableCollection<Sourate>(filtered);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void FilterSourates(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilteredSourates = new ObservableCollection<Sourate>(_allSourates);
                return;
            }

            var filtered = _allSourates.Where(s =>
                s.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                s.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                s.Id.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) 
               

            ).ToList();

            FilteredSourates = new ObservableCollection<Sourate>(filtered);
        }


        #region methodes privés
        private void GetAllSourates()
        {
            _allSourates = new ObservableCollection<Sourate>
        {
            new Sourate { Id = 1, Name = "سُورَةُ اُ۬لْفَاتِحَةِ", TotalVerses = 7, IsMecca = true },
            new Sourate { Id = 2, Name = "سُورَةُ اُ۬لْبَقَرَةِ", TotalVerses = 286, IsMecca = false },
            new Sourate { Id = 3, Name = "سورة آل عمران", TotalVerses = 200, IsMecca = false },
            new Sourate { Id = 4, Name = "سورة النساء", TotalVerses = 176, IsMecca = false },
            new Sourate { Id = 5, Name = "سورة المائدة", TotalVerses = 120, IsMecca = false },
            new Sourate { Id = 6, Name = "سورة الأنعام", TotalVerses = 165, IsMecca = true },
            new Sourate { Id = 7, Name = "سورةالأعراف", TotalVerses = 206, IsMecca = true },
            new Sourate { Id = 8, Name = "سورةالأنفال", TotalVerses = 75, IsMecca = false },
            new Sourate { Id = 9, Name = "سورة التوبة", TotalVerses = 129, IsMecca = false },
            new Sourate { Id = 10, Name = "سورة يونس", TotalVerses = 109, IsMecca = true },
            new Sourate { Id = 11, Name = "سورة هود", TotalVerses = 123, IsMecca = true },
            new Sourate { Id = 12, Name = "سورة يوسف", TotalVerses = 111, IsMecca = true },
            new Sourate { Id = 13, Name = "سورة الرعد", TotalVerses = 43, IsMecca = false },
            new Sourate { Id = 14, Name = "سورة إبراهيم", TotalVerses = 52, IsMecca = true },
            new Sourate { Id = 15, Name = "سورة الحجر", TotalVerses = 99, IsMecca = true },
            new Sourate { Id = 16, Name = "سورة النحل", TotalVerses = 128, IsMecca = true },
            new Sourate { Id = 17, Name = "سورة الإسراء", TotalVerses = 111, IsMecca = true },
            new Sourate { Id = 18, Name = "سورة الكهف", TotalVerses = 110, IsMecca = true },
            new Sourate { Id = 19, Name = "سورة مريم", TotalVerses = 98, IsMecca = true },
            new Sourate { Id = 20, Name = "سورة طـه", TotalVerses = 135, IsMecca = true },
            new Sourate { Id = 21, Name = "سورة الأنبياء", TotalVerses = 112, IsMecca = true },
            new Sourate { Id = 22, Name = "سورة الحج", TotalVerses = 78, IsMecca = false },
            new Sourate { Id = 23, Name = "سورة المؤمنون", TotalVerses = 118, IsMecca = true },
            new Sourate { Id = 24, Name = "سورة النور", TotalVerses = 64, IsMecca = false },
            new Sourate { Id = 25, Name = "سورة الفرقان", TotalVerses = 77, IsMecca = true },
            new Sourate { Id = 26, Name = "سورة الشعراء", TotalVerses = 227, IsMecca = true },
            new Sourate { Id = 27, Name = "سورة النمل", TotalVerses = 93, IsMecca = true },
            new Sourate { Id = 28, Name = "سورة القصص", TotalVerses = 88, IsMecca = true },
            new Sourate { Id = 29, Name = "سورة العنكبوت", TotalVerses = 69, IsMecca = true },
            new Sourate { Id = 30, Name = "سورة الروم", TotalVerses = 60, IsMecca = true },
            new Sourate { Id = 31, Name = "سورة لقمان", TotalVerses = 34, IsMecca = true },
            new Sourate { Id = 32, Name = "سورة السجدة", TotalVerses = 30, IsMecca = true },
            new Sourate { Id = 33, Name = "سورة الأحزاب", TotalVerses = 73, IsMecca = false },
            new Sourate { Id = 34, Name = "سورة سبأ", TotalVerses = 54, IsMecca = true },
            new Sourate { Id = 35, Name = "سورة فاطر", TotalVerses = 45, IsMecca = true },
            new Sourate { Id = 36, Name = "سورة يس", TotalVerses = 83, IsMecca = true },
            new Sourate { Id = 37, Name = "سورة الصافات", TotalVerses = 182, IsMecca = true },
            new Sourate { Id = 38, Name = "سورة ص", TotalVerses = 88, IsMecca = true },
            new Sourate { Id = 39, Name = "سورة الزمر", TotalVerses = 75, IsMecca = true },
            new Sourate { Id = 40, Name = "سورة غافر", TotalVerses = 85, IsMecca = true },
            new Sourate { Id = 41, Name = "سورة فصلت", TotalVerses = 54, IsMecca = true },
            new Sourate { Id = 42, Name = "سورة الشورى", TotalVerses = 53, IsMecca = true },
            new Sourate { Id = 43, Name = "سورة الزخرف", TotalVerses = 89, IsMecca = true },
            new Sourate { Id = 44, Name = "سورة الدخان", TotalVerses = 59, IsMecca = true },
            new Sourate { Id = 45, Name = "سورة الجاثية", TotalVerses = 37, IsMecca = true },
            new Sourate { Id = 46, Name = "سورة الأحقاف", TotalVerses = 35, IsMecca = true },
            new Sourate { Id = 47, Name = "سورة محمد", TotalVerses = 38, IsMecca = false },
            new Sourate { Id = 48, Name = "سورة الفتح", TotalVerses = 29, IsMecca = false },
            new Sourate { Id = 49, Name = "سورة الحجرات", TotalVerses = 18, IsMecca = false },
            new Sourate { Id = 50, Name =  "سورة ق", TotalVerses = 45, IsMecca = true },
            new Sourate { Id = 51, Name = "سورة الذاريات", TotalVerses = 60, IsMecca = true },
            new Sourate { Id = 52, Name = "سورة الطور", TotalVerses = 49, IsMecca = true },
            new Sourate { Id = 53, Name = "سورة النجم", TotalVerses = 62, IsMecca = true },
            new Sourate { Id = 54, Name = "سورة القمر", TotalVerses = 55, IsMecca = true },
            new Sourate { Id = 55, Name = "سورة الرحمن", TotalVerses = 78, IsMecca = false },
            new Sourate { Id = 56, Name = "سورة الواقعة", TotalVerses = 96, IsMecca = true },
            new Sourate { Id = 57, Name = "سورة الحديد", TotalVerses = 29, IsMecca = false },
            new Sourate { Id = 58, Name = "سورة المجادلة", TotalVerses = 22, IsMecca = false },
            new Sourate { Id = 59, Name = "سورة الحشر", TotalVerses = 24, IsMecca = false },
            new Sourate { Id = 60, Name = "سورة الممتحنة", TotalVerses = 13, IsMecca = false },
            new Sourate { Id = 61, Name = "سورة الصف", TotalVerses = 14, IsMecca = false },
            new Sourate { Id = 62, Name = "سورة الجمعة", TotalVerses = 11, IsMecca = false },
            new Sourate { Id = 63, Name = "سورة المنافقون", TotalVerses = 11, IsMecca = false },
            new Sourate { Id = 64, Name = "سورة التغابن", TotalVerses = 18, IsMecca = false },
            new Sourate { Id = 65, Name = "سورة الطلاق", TotalVerses = 12, IsMecca = false },
            new Sourate { Id = 66, Name = "سورة التحريم", TotalVerses = 12, IsMecca = false },
            new Sourate { Id = 67, Name = "سورة الملك", TotalVerses = 30, IsMecca = true },
            new Sourate { Id = 68, Name = "سورة القلم", TotalVerses = 52, IsMecca = true },
            new Sourate { Id = 69, Name = "سورة الحاقة", TotalVerses = 52, IsMecca = true },
            new Sourate { Id = 70, Name = "سورة المعارج", TotalVerses = 44, IsMecca = true },
            new Sourate { Id = 71, Name = "سورة نوح", TotalVerses = 28, IsMecca = true },
            new Sourate { Id = 72, Name = "سورة الجن", TotalVerses = 28, IsMecca = true },
            new Sourate { Id = 73, Name = "سورة المزمل", TotalVerses = 20, IsMecca = true },
            new Sourate { Id = 74, Name = "سورة المدثر", TotalVerses = 56, IsMecca = true },
            new Sourate { Id = 75, Name = "سورة القيامة", TotalVerses = 40, IsMecca = true },
            new Sourate { Id = 76, Name = "سورة الإنسان", TotalVerses = 31, IsMecca = false },
            new Sourate { Id = 77, Name = "سورة المرسلات", TotalVerses = 50, IsMecca = true },
            new Sourate { Id = 78, Name = "سورة النبأ", TotalVerses = 40, IsMecca = true },
            new Sourate { Id = 79, Name = "سورة النازعات", TotalVerses = 46, IsMecca = true },
            new Sourate { Id = 80, Name = "سورة عبس", TotalVerses = 42, IsMecca = true },
            new Sourate { Id = 81, Name = "سورة التكوير", TotalVerses = 29, IsMecca = true },
            new Sourate { Id = 82, Name = "سورة الإنفطار", TotalVerses = 19, IsMecca = true },
            new Sourate { Id = 83, Name = " سورة المطففين", TotalVerses = 36, IsMecca = true },
            new Sourate { Id = 84, Name = "سورة الإنشقاق", TotalVerses = 25, IsMecca = true },
            new Sourate { Id = 85, Name = "سورة البرج", TotalVerses = 22, IsMecca = true },
            new Sourate { Id = 86, Name = "سورة الطارق", TotalVerses = 17, IsMecca = true },
            new Sourate { Id = 87, Name = "سُورَةُ اُ۬لَاعْلَي", TotalVerses = 19, IsMecca = true },
            new Sourate { Id = 88, Name = "سورة الغاشية", TotalVerses = 26, IsMecca = true },
            new Sourate { Id = 89, Name = "سورة الفجر", TotalVerses = 30, IsMecca = true },
            new Sourate { Id = 90, Name = "سورة البلد", TotalVerses = 20, IsMecca = true },
            new Sourate { Id = 91, Name = "سورة الشمس", TotalVerses = 15, IsMecca = false },
            new Sourate { Id = 92, Name = "سورة الليل", TotalVerses = 21, IsMecca = true },
            new Sourate { Id = 93, Name = "سورة الضحى", TotalVerses = 11, IsMecca = true },
            new Sourate { Id = 94, Name = "سُورَةُ اُ۬لشَّرْحِ", TotalVerses = 8, IsMecca = true },
            new Sourate { Id = 95, Name = "سورة التين", TotalVerses = 8, IsMecca = true },
            new Sourate { Id = 96, Name = "سورة العلق", TotalVerses = 19, IsMecca = true },
            new Sourate { Id = 97, Name = "سورة القدر", TotalVerses = 5, IsMecca = true },
            new Sourate { Id = 98, Name = "سورة البيِّنة", TotalVerses = 8, IsMecca = false },
            new Sourate { Id = 99, Name = "سورة الزلزلة", TotalVerses = 8, IsMecca = false },
            new Sourate { Id = 100, Name = "سُورَةُ اُ۬لْعَٰدِيَٰتِ ", TotalVerses = 11, IsMecca = true },
            new Sourate { Id = 101, Name = "سورة القارعة", TotalVerses = 11, IsMecca = true },
            new Sourate { Id = 102, Name = "سورة التكاثر", TotalVerses = 8, IsMecca = true },
            new Sourate { Id = 103, Name = "سورة العصر", TotalVerses = 3, IsMecca = true },
            new Sourate { Id = 104, Name = "سورة الهمزة", TotalVerses = 9, IsMecca = true },
            new Sourate { Id = 105, Name = "سورة الفيل", TotalVerses = 5, IsMecca = true },
            new Sourate { Id = 106, Name = "سورة قريش", TotalVerses = 4, IsMecca = true },
            new Sourate { Id = 107, Name = "سُورَةُ اُ۬لْمَاعُونِ", TotalVerses = 6, IsMecca = true },
            new Sourate { Id = 108, Name = "سُورَةُ اُ۬لْكَوْثَرِ", TotalVerses = 3, IsMecca = true },
            new Sourate { Id = 109, Name = "سورة الكافرون", TotalVerses = 6, IsMecca = true },
            new Sourate { Id = 110, Name = "سُورَةُ اُ۬لنَّصْرِ", TotalVerses = 3, IsMecca = false },
            new Sourate { Id = 111, Name = "سورة المسد", TotalVerses = 5, IsMecca = true },
            new Sourate { Id = 112, Name = "سورة الإخلاص", TotalVerses = 4, IsMecca = true },
            new Sourate { Id = 113, Name = "سُورَةُ اُ۬لْفَلَقِ", TotalVerses = 5, IsMecca = true },
            new Sourate { Id = 114, Name = "سُورَةُ اُ۬لنَّاسِ", TotalVerses = 6, IsMecca = true }
        };
        }
        #endregion

    }
}
