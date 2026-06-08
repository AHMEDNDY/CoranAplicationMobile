using CoranWarshSynchroniser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoranWarshSynchroniser.ViewModels
{
    public class QuranViewModel : INotifyPropertyChanged 
    { 
        public ObservableCollection<QuranPageModel> Pages { get; set; } 
        public QuranViewModel(int startPage, object jsonData, bool shouldHighlight, string highlightVerse)
        {
            Pages = new ObservableCollection<QuranPageModel>();
            for (int i = 1; i <= 604; i++) 
            { 
                Pages.Add(new QuranPageModel 
                { 
                    PageNumber = i, 
                    JsonData = jsonData, 
                    HighlightVerse = highlightVerse, 
                    ShouldHighlight = shouldHighlight,
                    PageLabel = $"Page {i}", 
                    TopSpacing = (i == 1 || i == 2) ? 150 : 0 });
            }
            StartHighlightBlink(); 
        } 
        async void StartHighlightBlink() 
        { 
            for (int i = 0; i < 4; i++) 
            { 
                foreach (var p in Pages) p.ShouldHighlight = false;
                await Task.Delay(200);
                foreach (var p in Pages) 
                    p.ShouldHighlight = true;
                await Task.Delay(400); 
            } 
            foreach (var p in Pages) 
            {
                p.ShouldHighlight = false;
                p.HighlightVerse = "";
            } 
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
