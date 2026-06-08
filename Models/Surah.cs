using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CoranWarshSynchroniser.Models
{
    public class Surah : INotifyPropertyChanged
    {
        private Color _ayahBackgroundColor = Colors.Transparent;
        string _text;
        private bool _isMecca;
        [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("jozz")]
            public int Jozz { get; set; }

            [JsonProperty("page")]
            public string Page { get; set; }

            [JsonProperty("sura_no")]
            public int SuraNo { get; set; }

            [JsonProperty("sura_name_en")]
            public string SuraNameEn { get; set; }

            [JsonProperty("sura_name_ar")]
            public string SuraNameAr { get; set; }

            [JsonProperty("total_verses")]
            public int TotalVerses { get; set; }
        private Color _backgroundColor = Colors.Transparent;
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }
        public Color AyahBackgroundColor
        {
            get => _ayahBackgroundColor;
            set
            {
                _ayahBackgroundColor = value;
                OnPropertyChanged(nameof(AyahBackgroundColor));
            }
        }
        private Color _textColor = Colors.Black;
        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                OnPropertyChanged(nameof(TextColor));
            }
        }

        private Color _translationColor = Color.FromArgb("#6E6E6E");
        public Color TranslationColor
        {
            get => _translationColor;
            set
            {
                _translationColor = value;
                OnPropertyChanged(nameof(TranslationColor));
            }
        }

        [JsonProperty("is_mecca")]

        public bool IsMecca
        {
            get => _isMecca;
            set
            {
                if (_isMecca != value)
                {
                    _isMecca = value;
                    OnPropertyChanged(nameof(IsMecca));
                }
            }
        }

        [JsonProperty("aya_no")]
            public int AyaNo { get; set; }

            [JsonProperty("aya_text")]
            public string AyaText
            {
                get => _text;
                set
                {
                    if (_text == value)
                        return;
                    _text = value; OnPropertyChanged(nameof(AyaText)); OnPropertyChanged(nameof(AyaText));
                }
            }
        [JsonProperty("audio_path")]
        public string AudioPath { get; set; }

        bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value)
                    return;
                _isActive = value; OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(AyaText));
            }
        }
        //public FormattedString FormattedText
        //{
        //    get
        //    {
        //        var fs = new FormattedString(); fs.Spans.Add(new Span { Text = AyaText + " ", FontFamily = "UthmanicWarsh", FontSize = 32, TextColor = IsActive ? Colors.DarkGreen : Colors.Black }); fs.Spans.Add(new Span
        //        {
        //            Text = " ",

        //        });
        //        fs.Spans.Add(new Span { Text = "", TextColor = Colors.Transparent, });
        //        return fs;
        //    }
        //}

        [JsonPropertyName("start")]
        public double Start { get; set; }   // en secondes
        [JsonPropertyName("end")]
        public double End { get; set; }
        public bool IsHighlighted { get;  set; }
        public bool ShowTranslation { get;  set; }
        [JsonProperty("translation")]
        public string? Translation { get;  set; }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    }

