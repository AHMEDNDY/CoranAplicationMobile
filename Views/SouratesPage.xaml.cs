using CoranWarshSynchroniser.Models;
using CoranWarshSynchroniser.ViewModels;

namespace CoranWarshSynchroniser.Views;

public partial class SouratesPage : ContentPage
{
	public SouratesPage()
	{
		InitializeComponent();
        BindingContext = new SouratesViewModel();
    }
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is SouratesViewModel viewModel)
        {
            viewModel.FilterSourates(e.NewTextValue);
        }
    }
    
}