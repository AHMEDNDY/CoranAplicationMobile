namespace CoranWarshSynchroniser.Views
{
    [QueryProperty(nameof(CategoryId), "CategoryId")]
    [QueryProperty(nameof(CategoryName), "CategoryName")]
    public partial class DuaDetailPage : ContentPage
    {
        private int _categoryId;
        private string _categoryName = string.Empty;

        public int CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = value;
                OnPropertyChanged();
            }
        }

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged();
            }
        }

        public DuaDetailPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}