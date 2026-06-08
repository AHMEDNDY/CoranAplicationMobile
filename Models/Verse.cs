using MvvmHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoranWarshSynchroniser.Models
{
    public class Verse :  INotifyPropertyChanged
    {
        string _text;
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("text")]
        public string Text 
        { 
            get => _text; 
            set { 
                if (_text == value) 
                    return;
                _text = value; OnPropertyChanged(nameof(Text)); OnPropertyChanged(nameof(FormattedText));
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
                OnPropertyChanged(nameof(FormattedText)); 
            } 
        }
        public FormattedString FormattedText
        {
            get
            {
                var fs = new FormattedString(); fs.Spans.Add(new Span { Text = Text + " ", FontFamily = "UthmanicWarsh", FontSize = 32, TextColor = IsActive ? Colors.DarkGreen : Colors.Black }); fs.Spans.Add(new Span
                {
                    Text = " ", 
                   
                    });
                    fs.Spans.Add(new Span { Text = "", TextColor = Colors.Transparent, });
                     return fs;
            }
                }
            public event PropertyChangedEventHandler PropertyChanged; 
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
