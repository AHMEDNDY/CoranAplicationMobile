using CoranWarshSynchroniser.Models;
using CoranWarshSynchroniser.Services;
using CoranWarshSynchroniser.ViewModels;

namespace CoranWarshSynchroniser.Views;

public partial class SouratesPage : ContentPage
{
	public SouratesPage()
	{
		InitializeComponent();
        BindingContext = new SouratesViewModel(new SurahService());
    }
    
    
}