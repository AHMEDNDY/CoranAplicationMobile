
using System.Text.Json.Serialization;

namespace CoranWarshSynchroniser.Models;

public class AyahSource
{
    [JsonPropertyName("sura_no")]
    public int SuraNo { get; set; }

    [JsonPropertyName("sura_name_ar")]
    public string SuraNameAr { get; set; } = "";

    [JsonPropertyName("aya_no")]
    public int AyaNo { get; set; }
}
