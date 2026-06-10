
using Newtonsoft.Json;
using System.ComponentModel;

namespace CoranWarshSynchroniser.Models
{
    public class Sourate : INotifyPropertyChanged
    {
        
        private bool _isMecca;

        private bool _isFavorite;

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sura_name_en")]
        public string SuraNameEn { get; set; }

        [JsonProperty("transliteration")]
        public string Transliteration { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("total_verses")]
        public int TotalVerses { get; set; }

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
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                _isFavorite = value;
                OnPropertyChanged(nameof(HeartIcon));
            }
        }

        public string OriginIcon => IsMecca ? "mecca.png" : "medina.png";

        public string NumberBackgroundColor => IsMecca ? "#000000" : "#228B22";
        [JsonProperty("verses")]
        public List<Verse> Verses { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string HeartIcon => IsFavorite ? "❤️" : "🤍";
    }
}
