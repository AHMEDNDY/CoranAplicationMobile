using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoranWarshSynchroniser.Models
{
    public class QuranPageModel : INotifyPropertyChanged
    {
        public int PageNumber { get; set; }
        public object JsonData { get; set; }
        public string SurahName { get; set; }
        public string PageLabel { get; set; }
        public int TopSpacing { get; set; }

        string highlightVerse;
        public string HighlightVerse
        {
            get => highlightVerse;
            set { highlightVerse = value; OnPropertyChanged(); }
        }

        bool shouldHighlight;
        public bool ShouldHighlight
        {
            get => shouldHighlight;
            set { shouldHighlight = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
