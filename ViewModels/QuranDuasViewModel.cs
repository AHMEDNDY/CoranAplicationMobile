using CoranWarshSynchroniser.Models;
using CoranWarshSynchroniser.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CoranWarshSynchroniser.ViewModels
{
    public class QuranDuasViewModel : BaseViewModel
    {
        private readonly QuranDuaService _duaService = new();

        public ObservableCollection<QuranDua> Duas { get; } = new();

        public ICommand OpenDuaCommand { get; }

        public QuranDuasViewModel()
        {
            OpenDuaCommand = new Command<QuranDua>(
                async dua => await OpenDuaAsync(dua));
        }

        public async Task LoadDuasAsync()
        {
            var duas = await _duaService.GetDuasAsync();

            Duas.Clear();

            foreach (var dua in duas)
                Duas.Add(dua);
        }

        private async Task OpenDuaAsync(QuranDua dua)
        {
            if (dua == null)
                return;

            await Shell.Current.GoToAsync(
                nameof(Views.QuranReaderPage),
                new Dictionary<string, object>
                {
                    { "SourateId", dua.SuraNo },
                    { "AyaNo", dua.AyaNo }
                });
        }
    }
}
