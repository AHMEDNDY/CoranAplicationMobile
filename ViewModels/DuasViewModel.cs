using CoranWarshSynchroniser.Models;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Input;

namespace CoranWarshSynchroniser.ViewModels
{
    public class DuasViewModel : BaseViewModel
    {
        private readonly List<DuaCategory> _allCategories = new();

        private string _searchText = string.Empty;

        public ObservableCollection<DuaCategory> Categories { get; }
            = new();

        public ICommand CategoryTappedCommand { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value ?? string.Empty))
                {
                    FilterCategories();
                }
            }
        }

        public DuasViewModel()
        {
            CategoryTappedCommand =
                new Command<DuaCategory>(
                    async category =>
                        await OpenCategoryAsync(category));

            LoadCategories();
        }

        private void LoadCategories()
        {
            _allCategories.Clear();

            _allCategories.AddRange(new[]
            {
                new DuaCategory
                {
                    Id = 1,
                    Name = "أدعية من القرآن الكريم",
                    Icon = "📖",
                    Description = "أدعية قرآنية"
                },
                new DuaCategory
                {
                    Id = 2,
                    Name = "أدعية من السنة النبوية",
                    Icon = "🕌",
                    Description = "أدعية نبوية"
                },
                new DuaCategory
                {
                    Id = 3,
                    Name = "أدعية الصباح والمساء",
                    Icon = "🌅",
                    Description = "أذكار الصباح والمساء"
                },
                new DuaCategory
                {
                    Id = 4,
                    Name = "أدعية النوم",
                    Icon = "🌙",
                    Description = "أذكار النوم"
                },
                new DuaCategory
                {
                    Id = 5,
                    Name = "أدعية السفر",
                    Icon = "✈️",
                    Description = "دعاء السفر"
                },
                new DuaCategory
                {
                    Id = 6,
                    Name = "أدعية للحفظ والرزق",
                    Icon = "🤲",
                    Description = "أدعية الرزق والحفظ"
                },
                new DuaCategory
                {
                    Id = 7,
                    Name = "أدعية عامة",
                    Icon = "✿",
                    Description = "أدعية متنوعة"
                }
            });

            FilterCategories();
        }

        private void FilterCategories()
        {
            var search = NormalizeArabic(SearchText.Trim());

            var results = string.IsNullOrWhiteSpace(search)
                ? _allCategories
                : _allCategories
                    .Where(x =>
                        NormalizeArabic(x.Name).Contains(search) ||
                        NormalizeArabic(x.Description).Contains(search))
                    .ToList();

            Categories.Clear();

            foreach (var item in results)
                Categories.Add(item);
        }

        private static string NormalizeArabic(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalized = text.Normalize(NormalizationForm.FormD);

            var builder = new StringBuilder();

            foreach (char c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);

                if (category == UnicodeCategory.NonSpacingMark ||
                    category == UnicodeCategory.SpacingCombiningMark)
                    continue;

                builder.Append(c switch
                {
                    'أ' or 'إ' or 'آ' or 'ٱ' => 'ا',
                    'ى' => 'ي',
                    'ؤ' => 'و',
                    'ئ' => 'ي',
                    _ => c
                });
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        private async Task OpenCategoryAsync(DuaCategory? category)
        {
            if (category == null)
                return;

            if (category.Id == 1)
            {
                await Shell.Current.GoToAsync(
                    nameof(Views.QuranDuasPage));

                return;
            }

            await Shell.Current.GoToAsync(
                nameof(Views.DuaDetailPage),
                new Dictionary<string, object>
                {
                    ["CategoryId"] = category.Id,
                    ["CategoryName"] = category.Name
                });
        }
    }
}
