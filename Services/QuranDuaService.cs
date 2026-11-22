using CoranWarshSynchroniser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoranWarshSynchroniser.Services
{
    public class QuranDuaService
    {
        public async Task<List<QuranDua>> GetDuasAsync()
        {
            // قراءة قائمة الأدعية
            using var duasStream =
                await FileSystem.OpenAppPackageFileAsync(
                    "quran_duas.json");

            var duas = await JsonSerializer.DeserializeAsync<List<QuranDua>>(
                duasStream) ?? new();

            // قراءة القرآن
            using var quranStream =
                await FileSystem.OpenAppPackageFileAsync(
                    "surahs.json");

            var ayahs =
                await JsonSerializer.DeserializeAsync<List<AyahSource>>(
                    quranStream) ?? new();

            foreach (var dua in duas)
            {
                int endAya = dua.EndAyaNo ?? dua.AyaNo;

                var selectedAyahs = ayahs
                    .Where(a =>
                        a.SuraNo == dua.SuraNo &&
                        a.AyaNo >= dua.AyaNo &&
                        a.AyaNo <= endAya)
                    .OrderBy(a => a.AyaNo)
                    .ToList();

                dua.AyaText = string.Join(
                    " ۞ ",
                    selectedAyahs.Select(a => a.SuraNameAr));
            }

            return duas;
        }
    }
}
