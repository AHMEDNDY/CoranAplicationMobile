using CoranWarshSynchroniser.Views;

namespace CoranWarshSynchroniser
{
    public partial class AppShell : Shell
    {
        

        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute("TesbihPage", typeof(TesbihPage));
            //Routing.RegisterRoute("SouratesPage", typeof(SouratesPage));
             Routing.RegisterRoute(nameof(QuranReaderPage), typeof(QuranReaderPage));
            // Pages déjà présentes dans votre projet
            Routing.RegisterRoute(nameof(SouratesPage), typeof(SouratesPage));
            Routing.RegisterRoute(nameof(AudioPage), typeof(AudioPage));
            Routing.RegisterRoute(nameof(TesbihPage), typeof(TesbihPage));

            // Petites pages incluses dans ce paquet de démonstration
            Routing.RegisterRoute(nameof(DuaHubPage), typeof(DuaHubPage));
            Routing.RegisterRoute(nameof(FavoriteHubPage), typeof(FavoriteHubPage));
            Routing.RegisterRoute(nameof(QuranSettingsPage), typeof(QuranSettingsPage));
            Routing.RegisterRoute(
               nameof(DuaDetailPage),
               typeof(DuaDetailPage));

            Routing.RegisterRoute(
                nameof(DuasPage),
                typeof(DuasPage));
            Routing.RegisterRoute(
    nameof(QuranDuasPage),
    typeof(QuranDuasPage));
            // Conservez ici vos autres enregistrements de routes existants.
            // Ne doublez pas un RegisterRoute déjà présent dans votre projet.
        }
    }
}
