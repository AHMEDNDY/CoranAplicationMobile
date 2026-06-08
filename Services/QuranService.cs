using CoranWarshSynchroniser.Models;
using System.Text.Json;

namespace CoranWarshSynchroniser.Services
{
    public class QuranService
    {
        public async Task<List<Ayah>> LoadAsync(string file)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(file);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            return JsonSerializer.Deserialize<List<Ayah>>(json);
        }
    }
}
