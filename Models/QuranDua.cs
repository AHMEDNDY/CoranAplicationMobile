using System.Text.Json.Serialization;

namespace CoranWarshSynchroniser.Models
{
    public class QuranDua
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("sura_no")]
        public int SuraNo { get; set; }

        [JsonPropertyName("aya_no")]
        public int AyaNo { get; set; }

        [JsonPropertyName("end_aya_no")]
        public int? EndAyaNo { get; set; }

        public string AyaText { get; set; } = "";

        public string Reference =>
            $"سورة {SuraNo} - الآية {AyaNo}";
    }
}
