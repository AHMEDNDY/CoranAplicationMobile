using CoranWarshSynchroniser.Models;

namespace CoranWarshSynchroniser.Views;

public partial class QuranPage : ContentPage
{
    public List<Surah> SurahJsonData { get; set; }
    private List<Surah> _originalList;

    public QuranPage(List<Surah> suraJsonData) 
    { 
        SurahJsonData = suraJsonData; 
        Content = new VerticalStackLayout { Children = { new Label { Text = "Quran Page", FontSize = 24 } } }; }
    private async void OnSurahSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Surah selected)
        {
            // Navigation vers ta page mushaf
            await Navigation.PushAsync(new QuranViewPage(
                pageNumber: 1,
                jsonData: SurahJsonData,
                false,
                highlightVerse: ""
            ));
        }
    }
}
