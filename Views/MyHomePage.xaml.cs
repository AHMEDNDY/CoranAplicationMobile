using CoranWarshSynchroniser.Models;
using CoranWarshSynchroniser.Services;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Linq;

namespace CoranWarshSynchroniser.Views;

public partial class MyHomePage : ContentPage
{
    readonly QuranService _quranService;
    private readonly AudioService _audioService;
    public List<Surah> SurahJsonData { get; set; }
    private Surah _currentAyah;
    public MyHomePage()
    {
        InitializeComponent();
        BindingContext = this; // essentiel pour le XAML
        LoadSurah(1);
    }
    //public Surah CurrentAyah
    //{
    //    get => _currentAyah;
    //    set => SetProperty(ref _currentAyah, value);
    //}
    async void LoadSurah(int surahNumber)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("surahs.json");
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        var SurahJsonData = JsonConvert.DeserializeObject<List<Surah>>(json);

 

        var surahAyat = SurahJsonData
            .Where(a => a.SuraNo == surahNumber)
            .OrderBy(a => a.SuraNo)
            .ToList();

        StringBuilder sb = new StringBuilder();

        foreach (var ayah in surahAyat)
        {
            // Supprimer le symbole unicode ﰀ si présent
            //string cleanText;
                //= ayah.AyaText.Replace("\uF000", "").Trim();

            //sb.Append(cleanText);
            //sb.Append(" ۝");
            sb.Append(ayah.AyaText);
            sb.Append(" ");
            //sb.Append(" ");
        }

        QuranTextLabel.Text = sb.ToString();

     
    }


    private async void LoadJsonAsset()
    {

        var ayat = new List<string>()
        {
            "يَا أَيُّهَا النَّاسُ اتَّقُوا رَبَّكُمُ",
            "الَّذِي خَلَقَكُم مِّن نَّفْسٍ وَاحِدَةٍ",
            "وَخَلَقَ مِنْهَا زَوْجَهَا"
        };

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < ayat.Count; i++)
        {
            sb.Append(ayat[i]);

            // numéro décoré comme mushaf
            sb.Append(" ۝");
            sb.Append(i + 1);
            sb.Append(" ");
        }

        QuranTextLabel.Text = sb.ToString();
        //try
        //{
        //    using var stream = await FileSystem.OpenAppPackageFileAsync("surahs.json");
        //    using var reader = new StreamReader(stream);
        //    var json = await reader.ReadToEndAsync();

        //    var SurahJsonData = JsonConvert.DeserializeObject<List<Surah>>(json);
        //    SurahListView.ItemsSource = SurahJsonData;

        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("JSON ERROR: " + ex.Message);
        //}
    }

    //private async void OnSurahSelected(object sender, SelectionChangedEventArgs e)
    //{
    //    if (e.CurrentSelection.FirstOrDefault() is Surah selectedSurah)
    //    {
    //        await Navigation.PushAsync(new QuranPage(selectedSurah));
    //    }
    //}
}
