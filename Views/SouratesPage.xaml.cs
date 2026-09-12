using CoranWarshSynchroniser.Services;
using CoranWarshSynchroniser.ViewModels;

namespace CoranWarshSynchroniser.Views;

public partial class SouratesPage : Views.AnimatedContentPage
{
	public SouratesPage()
	{
		InitializeComponent();
        BindingContext = new SouratesViewModel(new SurahService());
    }

}