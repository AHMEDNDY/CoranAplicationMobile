namespace CoranWarshSynchroniser.ViewModels;


public partial class QuranRichTextView : ContentView
{
    public static readonly BindableProperty PageNumberProperty =
        BindableProperty.Create(
            nameof(PageNumber),
            typeof(int),
            typeof(QuranRichTextView),
            1,
            propertyChanged: OnPropertyChanged);

    public int PageNumber
    {
        get => (int)GetValue(PageNumberProperty);
        set => SetValue(PageNumberProperty, value);
    }

    public static readonly BindableProperty JsonDataProperty =
        BindableProperty.Create(
            nameof(JsonData),
            typeof(object),
            typeof(QuranRichTextView),
            null,
            propertyChanged: OnPropertyChanged);

    public object JsonData
    {
        get => GetValue(JsonDataProperty);
        set => SetValue(JsonDataProperty, value);
    }

    public static readonly BindableProperty HighlightVerseProperty =
        BindableProperty.Create(
            nameof(HighlightVerse),
            typeof(string),
            typeof(QuranRichTextView),
            "",
            propertyChanged: OnPropertyChanged);

    public string HighlightVerse
    {
        get => (string)GetValue(HighlightVerseProperty);
        set => SetValue(HighlightVerseProperty, value);
    }

    public static readonly BindableProperty ShouldHighlightProperty =
        BindableProperty.Create(
            nameof(ShouldHighlight),
            typeof(bool),
            typeof(QuranRichTextView),
            false,
            propertyChanged: OnPropertyChanged);

    public bool ShouldHighlight
    {
        get => (bool)GetValue(ShouldHighlightProperty);
        set => SetValue(ShouldHighlightProperty, value);
    }

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (QuranRichTextView)bindable;
        control.RenderPage();
    }

    public QuranRichTextView()
    {
        Content = new Label
        {
            TextColor = Colors.Black,
            FontSize = 24,
            HorizontalTextAlignment = TextAlignment.Center
        };
    }

    void RenderPage()
    {
        var formatted = new FormattedString();

        // TEMP : données factices pour éviter les erreurs
        var pageData = new List<(int Surah, int Start, int End)>
        {
            (1, 1, 7)
        };

        foreach (var block in pageData)
        {
            for (int i = block.Start; i <= block.End; i++)
            {
                string verse = $"[{block.Surah}:{i}] ";

                var span = new Span
                {
                    Text = verse,
                    FontFamily = $"QCF_P{PageNumber:D3}",
                    TextColor = (ShouldHighlight && HighlightVerse == $"{block.Surah}:{i}")
                                ? Colors.Orange
                                : Colors.Black
                };

                formatted.Spans.Add(span);
            }
        }

        (Content as Label).FormattedText = formatted;
    }
}



