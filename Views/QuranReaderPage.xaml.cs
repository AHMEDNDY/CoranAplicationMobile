using CoranWarshSynchroniser.ViewModels;

namespace CoranWarshSynchroniser.Views;
[QueryProperty(nameof(SourateId), "SourateId")]

public partial class QuranReaderPage : ContentPage
{
    private readonly QuranReaderViewModel _vm;
    public int SourateId
    {
        set
        { // On transmet au ViewModel
            (BindingContext as QuranReaderViewModel).LoadSourate(value);
          } 
    }
   
    public QuranReaderPage()
    {
        InitializeComponent();
        _vm = new QuranReaderViewModel();
        BindingContext = _vm;

        _vm.ScrollToSpanRequested += ScrollToAyah;
    }
    private void ScrollToAyah(int index)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(100);

            QuranCollection.ScrollTo(
                index,
                position: ScrollToPosition.Start,
                animate: true);
        });
    }







}

