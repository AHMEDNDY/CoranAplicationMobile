using CoranWarshSynchroniser.ViewModels;

namespace CoranWarshSynchroniser.Views;

[QueryProperty(nameof(SourateId), "SourateId")]
public partial class QuranReaderPage : ContentPage
{
    private readonly QuranReaderViewModel _vm;

    private bool _isDraggingSlider;
    private bool _isProcessingSeek;

    private int _sourateId;

    public int SourateId
    {
        get => _sourateId;
        set
        {
            _sourateId = value;
            _ = _vm.LoadSourateAsync(value);
        }
    }

    public QuranReaderPage()
    {
        InitializeComponent();

        _vm = new QuranReaderViewModel();

        BindingContext = _vm;

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

                var ayah = _vm.SurahJsonData
                    .FirstOrDefault(a => a.AyaNo == ayaNo);

                if (ayah != null)
                {
                    QuranCollection.ScrollTo(
                        ayah,
                        position: ScrollToPosition.Center,
                        animate: true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"ScrollToAyah : {ex.Message}");
            }
        });
    }

    private void OnProgressDragStarted(
        object sender, EventArgs e)
    {
        _isDraggingSlider = true;

        _vm.BeginSeeking();
    }

    private void OnProgressValueChanged(
        object sender, ValueChangedEventArgs e)
    {
        if (!_isDraggingSlider || _isProcessingSeek)
            return;

        _vm.AudioPositionSeconds = e.NewValue;
    }

    private async void OnProgressDragCompleted(
        object sender, EventArgs e)
    {
        if (sender is not Slider slider)
            return;

        if (_isProcessingSeek)
            return;

        _isProcessingSeek = true;

        try
        {
            await _vm.SeekAsync(slider.Value);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"ProgressSlider : {ex.Message}");
        }
        finally
        {
            _isDraggingSlider = false;
            _isProcessingSeek = false;

            _vm.EndSeeking();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _vm.ScrollToSpanRequested -= ScrollToAyah;
        _vm.ScrollToSpanRequested += ScrollToAyah;

        _vm.StartSynchronization();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _vm.StopPlayer();

        _vm.ScrollToSpanRequested -= ScrollToAyah;
    }
}