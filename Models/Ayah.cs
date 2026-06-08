using MvvmHelpers;
using System.Text.Json.Serialization;

namespace CoranWarshSynchroniser.Models
{
    public class Ayah : BaseViewModel
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
        [JsonPropertyName("audioPath")]
        public string? AudioPath { get; set; }
    }
    }
