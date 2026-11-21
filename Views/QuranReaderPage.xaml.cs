using CoranWarshSynchroniser.ViewModels;

namespace CoranWarshSynchroniser.Views;

[QueryProperty(nameof(SourateId), "SourateId")]
public partial class QuranReaderPage : ContentPage
{
    private readonly QuranReaderViewModel _vm;

    // Shell définit cette propriété après la construction de la page.
    public int SourateId
    {
        set => _ = _vm.LoadSourateAsync(value);
    }

    public QuranReaderPage()
    {
        InitializeComponent();
        _vm = new QuranReaderViewModel();
        BindingContext = _vm;

        _vm.ScrollToSpanRequested += ScrollToAyah;
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);
    }

    private void ScrollToAyah(int ayaNo)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await Task.Delay(90);
                var ayah = _vm.SurahJsonData.FirstOrDefault(a => a.AyaNo == ayaNo);
                if (ayah != null)
                    QuranCollection.ScrollTo(ayah, position: ScrollToPosition.Center, animate: true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ScrollToAyah : {ex.Message}");
            }
        });
    }

    private void OnProgressDragStarted(object sender, EventArgs e) => _vm.BeginSeeking();

    private async void OnProgressDragCompleted(object sender, EventArgs e)
    {
        if (sender is Slider slider)
            await _vm.SeekAsync(slider.Value);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.StopPlayer();
        _vm.ScrollToSpanRequested -= ScrollToAyah;
    }
}
