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
    private void OnGoToAyahClicked(object sender, EventArgs e)
    {
        if (BindingContext is not QuranReaderViewModel viewModel ||
            viewModel.SurahJsonData is not { Count: > 0 } ayahs)
        {
            return;
        }

        int first = ayahs.Min(a => a.AyaNo);
        int last = ayahs.Max(a => a.AyaNo);

        GoToAyahEntry.Text = string.Empty;
        GoToAyahEntry.Placeholder = $"{first}–{last}";
        GoToAyahError.IsVisible = false;
        GoToAyahOverlay.IsVisible = true;

        Dispatcher.Dispatch(() =>
        {
            if (GoToAyahOverlay.IsVisible)
                GoToAyahEntry.Focus();
        });
    }

    private void OnConfirmGoToAyah(object sender, EventArgs e)
    {
        if (BindingContext is not QuranReaderViewModel viewModel ||
            viewModel.SurahJsonData is not { Count: > 0 } ayahs)
        {
            CloseGoToAyah();
            return;
        }

        string input = (GoToAyahEntry.Text ?? string.Empty).Trim();
        string normalized = string.Concat(input.Select(digit => digit switch
        {
            >= '٠' and <= '٩' => (char)('0' + digit - '٠'),
            >= '۰' and <= '۹' => (char)('0' + digit - '۰'),
            _ => digit
        }));

        if (int.TryParse(normalized, out int number))
        {
            var ayah = ayahs.FirstOrDefault(a => a.AyaNo == number);

            if (ayah is not null)
            {
                CloseGoToAyah();
                QuranCollection.ScrollTo(
                    ayah,
                    position: ScrollToPosition.Center,
                    animate: true);
                return;
            }
        }

        int first = ayahs.Min(a => a.AyaNo);
        int last = ayahs.Max(a => a.AyaNo);
        GoToAyahError.Text = $"أدخل رقم آية صحيحًا بين {first} و{last}.";
        GoToAyahError.IsVisible = true;
    }

    private void OnCancelGoToAyah(object sender, EventArgs e)
    {
        CloseGoToAyah();
    }

    private void CloseGoToAyah()
    {
        GoToAyahEntry.Unfocus();
        GoToAyahOverlay.IsVisible = false;
    }
    //private async void OnGoToAyahClicked(object sender, EventArgs e)
    //{
    //    if (BindingContext is not QuranReaderViewModel viewModel ||
    //        viewModel.SurahJsonData is not { Count: > 0 } ayahs)
    //    {
    //        return;
    //    }

    //    int firstAyah = ayahs.Min(ayah => ayah.AyaNo);
    //    int lastAyah = ayahs.Max(ayah => ayah.AyaNo);

    //    string? input = await DisplayPromptAsync(
    //        "اذهب إلى الآية",
    //        "أدخل رقم الآية",
    //        accept: "اذهب",
    //        cancel: "إلغاء",
    //        placeholder: $"{firstAyah}–{lastAyah}",
    //        maxLength: lastAyah.ToString().Length,
    //        keyboard: Keyboard.Numeric);

    //    if (input is null)
    //        return;

    //    // Accepte les chiffres 123, ١٢٣ et ۱۲۳.
    //    string normalized = string.Concat(input.Trim().Select(digit => digit switch
    //    {
    //        >= '٠' and <= '٩' => (char)('0' + digit - '٠'),
    //        >= '۰' and <= '۹' => (char)('0' + digit - '۰'),
    //        _ => digit
    //    }));

    //    if (int.TryParse(normalized, out int ayahNumber))
    //    {
    //        var target = ayahs.FirstOrDefault(ayah => ayah.AyaNo == ayahNumber);

    //        if (target is not null)
    //        {
    //            QuranCollection.ScrollTo(
    //                target,
    //                position: ScrollToPosition.Center,
    //                animate: true);

    //            return;
    //        }
    //    }

    //    await DisplayAlert(
    //        "رقم الآية غير صالح",
    //        $"أدخل رقمًا من {firstAyah} إلى {lastAyah}.",
    //        "حسنًا");
    //}
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