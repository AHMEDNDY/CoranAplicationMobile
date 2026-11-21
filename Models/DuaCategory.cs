using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoranWarshSynchroniser.Models
{
    public class DuaCategory : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _icon = string.Empty;

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }

        public string Description { get; set; } = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
