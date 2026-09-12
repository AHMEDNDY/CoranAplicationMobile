using CoranWarshSynchroniser.Views;

namespace CoranWarshSynchroniser
{
    public partial class AppShell : Shell
    {
        

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("TesbihPage", typeof(TesbihPage));
            Routing.RegisterRoute("SouratesPage", typeof(SouratesPage));
            Routing.RegisterRoute(nameof(QuranReaderPage), typeof(QuranReaderPage));
        }
    }
}
