namespace CoranWarshSynchroniser.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnReadQuranTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(SouratesPage));

    private async void OnListenTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(AudioPage));

    private async void OnDuaTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(DuasPage));

    private async void OnFavoriteTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(FavoriteHubPage));
}
