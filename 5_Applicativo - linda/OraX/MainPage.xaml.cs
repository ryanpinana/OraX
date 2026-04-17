namespace OraX
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            LoadTheme();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Statistiche));
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Modifiche));
        }
        private void OnThemeChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;

            switch (picker.SelectedIndex)
            {
                case 0:
                    ThemeManager.SetLightTheme();
                    Preferences.Set("AppTheme", "Light");
                    break;

                case 1:
                    ThemeManager.SetDarkTheme();
                    Preferences.Set("AppTheme", "Dark");
                    break;

                case 2:
                    ThemeManager.SetBlueTheme();
                    Preferences.Set("AppTheme", "Blue");
                    break;
                case 3:
                    ThemeManager.SetPinkTheme();
                    Preferences.Set("AppTheme", "Pink");
                    break;
                case 4:
                    ThemeManager.SetPurpleTheme();
                    Preferences.Set("AppTheme", "Purple");
                    break;
                case 5:
                    ThemeManager.SetYellowTheme();
                    Preferences.Set("AppTheme", "Yellow");
                    break;
                case 6:
                    ThemeManager.SetRedTheme();
                    Preferences.Set("AppTheme", "Red");
                    break;
                case 7:
                    ThemeManager.SetBrownTheme();
                    Preferences.Set("AppTheme", "Brown");
                    break;
                default:
                    ThemeManager.SetDefaultTheme();
                    Preferences.Set("AppTheme", "Default");
                    break;
            }

            ApplyTheme();
        }

        void LoadTheme()
        {
            string theme = Preferences.Get("AppTheme", "Default");

            switch (theme)
            {
                case "Dark":
                    ThemeManager.SetDarkTheme();
                    break;

                case "Blue":
                    ThemeManager.SetBlueTheme();
                    break;
                case "Pink":
                    ThemeManager.SetPinkTheme();
                    break;
                case "Purple":
                    ThemeManager.SetPurpleTheme();
                    break;
                case "Yellow":
                    ThemeManager.SetYellowTheme();
                    break;
                case "Red":
                    ThemeManager.SetRedTheme();
                    break;
                case "Brown":
                    ThemeManager.SetBrownTheme();
                    break;
                default:
                    ThemeManager.SetDefaultTheme();
                    break;
            }

            ApplyTheme();
        }

        void ApplyTheme()
        {
            // Sfondo pagina
            this.BackgroundColor = ThemeManager.BackgroundColor;

            // Testi
            titleLabel.TextColor = ThemeManager.TextColor;
            frameProfile.BackgroundColor = ThemeManager.FrameBackgroundColor;
            frameProfile.BorderColor = ThemeManager.FrameBorderColor;

            frameStatistiche.BackgroundColor = ThemeManager.FrameBackgroundColor;
            frameStatistiche.BorderColor = ThemeManager.FrameBorderColor;

            frameActions.BackgroundColor = ThemeManager.FrameBackgroundColor;
            frameActions.BorderColor = ThemeManager.FrameBorderColor;

            frameProfileImage.BackgroundColor = ThemeManager.FrameBackgroundColor;
            frameProfileImage.BorderColor = ThemeManager.FrameBorderColor;
            themePicker.TextColor = ThemeManager.TextColor;
        }
    }
}