namespace CoranWarshSynchroniser.Views
{
    public partial class DuasPage : ContentPage
    {
        public DuasPage()
        {
            InitializeComponent();
        }

        private async void OnBackTapped(
            object sender,
            TappedEventArgs e)
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}