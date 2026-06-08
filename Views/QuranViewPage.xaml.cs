using CoranWarshSynchroniser.ViewModels;

namespace CoranWarshSynchroniser.Views;

public partial class QuranViewPage : ContentPage
{
    QuranViewModel vm;
    public QuranViewPage(int pageNumber, object jsonData, bool shouldHighlight, string highlightVerse)
    { 
        InitializeComponent(); 
        vm = new QuranViewModel(pageNumber, jsonData, shouldHighlight, highlightVerse); 
        BindingContext = vm;
        DeviceDisplay.KeepScreenOn = true; 
    }
    void OnBackClicked(object sender, EventArgs e) { Navigation.PopAsync(); }
    protected override void OnDisappearing() { base.OnDisappearing(); DeviceDisplay.KeepScreenOn = false; }
}