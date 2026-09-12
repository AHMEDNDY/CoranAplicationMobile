namespace CoranWarshSynchroniser.Views;


public partial class CustomBottomNav : ContentView
{
    private readonly Color ActiveColor = Color.FromArgb("#4A90E2");   // vert actif
    private readonly Color InactiveColor = Colors.Transparent;

    public CustomBottomNav()
    {
        InitializeComponent();
        Loaded += (s, e) => SyncActiveTabWithCurrentRoute();
    }

    private void SyncActiveTabWithCurrentRoute()
    {
        var route = Shell.Current?.CurrentState?.Location?.OriginalString ?? "";

        if (route.Contains("SouratesPage")) SetActiveTab(TabQuran);
        else if (route.Contains("AudioPage")) SetActiveTab(TabCategory);
        else if (route.Contains("TesbihPage")) SetActiveTab(TabContributors);
    }
    private async void OnTabQuranTapped(object sender, EventArgs e)
    {
        await AnimateAndNavigate(TabQuran, "//SouratesPage");
    }

    private async void OnTabCategoryTapped(object sender, EventArgs e)
    {
        await AnimateAndNavigate(TabCategory, "//AudioPage");
    }

    private async void OnTabContributorsTapped(object sender, EventArgs e)
    {
        await AnimateAndNavigate(TabContributors, "//TesbihPage");
    }

 
    [Obsolete]
    private void SetActiveTab(Frame activeTab)
    {
        // Réinitialiser tous les onglets
        foreach (var tab in new[] { TabQuran, TabCategory, TabContributors })
        {
            tab.BackgroundColor = InactiveColor;
        }

        // Appliquer le style "actif" arrondi + ombre au bouton sélectionné
        activeTab.BackgroundColor = ActiveColor;
        activeTab.Shadow = new Shadow
        {
            Brush = Brush.Black,
            Offset = new Point(0, 3),
            Radius = 10,
            Opacity = 0.4f
        };
    }

    [Obsolete]
    private async Task AnimateAndNavigate(Frame tappedTab, string route)
    {
        SetActiveTab(tappedTab);

        // Animation du bouton
        await tappedTab.ScaleTo(0.88, 80, Easing.CubicOut);
        await tappedTab.ScaleTo(1.0, 120, Easing.SpringOut);

        // Fade-out léger de la page courante avant de naviguer
        if (Shell.Current?.CurrentPage != null)
        {
            var currentPage = Shell.Current.CurrentPage;
            await currentPage.FadeTo(0.4, 120, Easing.CubicIn);
        }

        await Shell.Current.GoToAsync(route);
    }
}