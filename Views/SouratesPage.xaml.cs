using CoranWarshSynchroniser.ViewModels;

namespace CoranWarshSynchroniser.Views;


    public partial class SouratesPage : AnimatedContentPage
    {
        private bool _surahsLoaded = false;

        public SouratesPage()
        {
            InitializeComponent();

            // Initialisation du ViewModel
            BindingContext = new SouratesViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Éviter de recharger plusieurs fois
            if (_surahsLoaded)
                return;

            if (BindingContext is SouratesViewModel vm)
            {
                await vm.LoadSurah();

                // Marquer comme chargé uniquement si des données existent
                _surahsLoaded = vm.Surahs.Count > 0;
            }
        }
        }
    
