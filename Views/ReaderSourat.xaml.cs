using CoranWarshSynchroniser.ViewModels;

namespace CoranWarshSynchroniser.Views;

public partial class ReaderSourat : ContentPage
{
    private readonly QuranReaderViewModel _vm;

    public int SourateId
    {
        set
        { // On transmet au ViewModel
            (BindingContext as QuranReaderViewModel).LoadSourate(value);
        }
    }

    public ReaderSourat()
    {
        InitializeComponent();
        BindingContext = new QuranReaderViewModel();
    }
}