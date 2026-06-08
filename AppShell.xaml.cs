using CoranWarshSynchroniser.Views;

namespace CoranWarshSynchroniser
{
    public partial class AppShell : Shell
    {
        

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("QuranReaderPage", typeof(QuranReaderPage));
        }
    }
}
