using Microsoft.Maui.ApplicationModel;
namespace CoranWarshSynchroniser.Views;


public partial class CustomBottomNav : ContentView
{
    public CustomBottomNav()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (Shell.Current is not null)
        {
            Shell.Current.Navigated -= ShellNavigated;
            Shell.Current.Navigated += ShellNavigated;
        }
        RefreshActiveTab();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        if (Shell.Current is not null)
            Shell.Current.Navigated -= ShellNavigated;
    }

    private void ShellNavigated(object? sender, ShellNavigatedEventArgs e)
        => MainThread.BeginInvokeOnMainThread(RefreshActiveTab);

    private void RefreshActiveTab()
    {
        string page = Shell.Current?.CurrentPage?.GetType().Name ?? "MainPage";
        string key = page switch
        {
            "MainPage" => "home",
            "DuaHubPage" => "dua",
            "FavoriteHubPage" => "favorite",
            "QuranSettingsPage" => "settings",
            "SouratesPage" or "AudioPage" => "quran",
            _ when page.Contains("Dua", StringComparison.OrdinalIgnoreCase) => "dua",
            _ when page.Contains("Favorite", StringComparison.OrdinalIgnoreCase) => "favorite",
            _ when page.Contains("Settings", StringComparison.OrdinalIgnoreCase) => "settings",
            _ => "quran"
        };

        (Border tab, Label label, Image image, string imageName, string id)[] items =
        {
            (TabQuran, TextQuran, IconQuran, "footer_quran", "quran"),
            (TabDua, TextDua, IconDua, "footer_dua", "dua"),
            (TabHome, TextHome, IconHome, "footer_home", "home"),
            (TabFavorites, TextFavorites, IconFavorites, "footer_favorite", "favorite"),
            (TabSettings, TextSettings, IconSettings, "footer_settings", "settings")
        };

        foreach (var item in items)
        {
            bool active = item.id == key;
            item.tab.BackgroundColor = active ? Color.FromArgb("#1557BC") : Colors.Transparent;
            item.tab.Stroke = new SolidColorBrush(active ? Color.FromArgb("#F1C665") : Colors.Transparent);
            item.label.TextColor = active ? Color.FromArgb("#FFD971") : Colors.White;
            item.image.Source = $"{item.imageName}_{(active ? "gold" : "white")}.png";
        }
    }

    // La route racine réinitialise l'historique de navigation.
    private async Task GoAsync(string route)
    {
        if (Shell.Current is null) return;
        if (route == "home")
        {
            await Shell.Current.GoToAsync("//home");
            return;
        }
        if (Shell.Current.CurrentPage?.GetType().Name == route) return;

        // Evite l'empilement des pages lorsque l'on passe d'un onglet à l'autre.
        await Shell.Current.GoToAsync("//home");
        await Shell.Current.GoToAsync(route);
    }

    private async void OnQuranTapped(object sender, TappedEventArgs e)
        => await GoAsync(nameof(SouratesPage));

    private async void OnDuaTapped(object sender, TappedEventArgs e)
        => await GoAsync(nameof(DuasPage));

    private async void OnHomeTapped(object sender, TappedEventArgs e)
        => await GoAsync("home");

    private async void OnFavoritesTapped(object sender, TappedEventArgs e)
        => await GoAsync(nameof(FavoriteHubPage));

    private async void OnSettingsTapped(object sender, TappedEventArgs e)
        => await GoAsync(nameof(QuranSettingsPage));
}
