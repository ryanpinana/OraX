namespace OraX
{
    public partial class MainPage : ContentPage
    {
        private DateTime _dataSelezionata = DateTime.Today;
        public DateTime DataSelezionata
        {
            get => _dataSelezionata;
            set
            {
                _dataSelezionata = value;
                OnPropertyChanged();
                DisplayAlert("Data Scelta", $"Hai selezionato: {value.ToShortDateString()}", "OK");
            }

        }
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        
    }
}
