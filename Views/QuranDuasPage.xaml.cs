using CoranWarshSynchroniser.ViewModels;

namespace CoranWarshSynchroniser.Views;


    public partial class QuranDuasPage : ContentPage
{
    public QuranDuasPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is QuranDuasViewModel vm)
            await vm.LoadDuasAsync();
    }
}
