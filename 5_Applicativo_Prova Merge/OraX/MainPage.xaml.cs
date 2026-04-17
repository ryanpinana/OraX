namespace OraX;

public partial class MainPage : ContentPage
{

    private User user;

    public MainPage()
    {
        InitializeComponent();

        user = UserSession.CurrentUser;

        if (user != null)
        {
            labelNome.Text = $"{user.Nome} {user.Cognome}";
            labelEmail.Text = user.Email;
        }

        LoadTheme();
        ThemeManager.ThemeChanged += ApplyTheme;
    }

  
    private async void ButtonStats_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Statistiche));
    }

 
    private async void ButtonModifica_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Modifiche));
    }

    // 🚪 LOGOUT
    private async void ButtonLogout_Clicked(object sender, EventArgs e)
    {
        UserSession.CurrentUser = null;
        await Shell.Current.GoToAsync("//LoginPage");
    }

    // 🎨 CAMBIO TEMA
    private void OnThemeChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;

        switch (picker.SelectedIndex)
        {
            case 0: ThemeManager.SetLightTheme(); break;
            case 1: ThemeManager.SetDarkTheme(); break;
            case 2: ThemeManager.SetBlueTheme(); break;
            case 3: ThemeManager.SetPinkTheme(); break;
            case 4: ThemeManager.SetPurpleTheme(); break;
            case 5: ThemeManager.SetYellowTheme(); break;
            case 6: ThemeManager.SetRedTheme(); break;
            case 7: ThemeManager.SetBrownTheme(); break;
            default: ThemeManager.SetDefaultTheme(); break;
        }

        Preferences.Set("AppTheme", picker.SelectedIndex.ToString());
        ApplyTheme();
    }

    // 🔁 CARICA TEMA
    void LoadTheme()
    {
        string theme = Preferences.Get("AppTheme", "0");

        if (int.TryParse(theme, out int index))
        {
            switch (index)
            {
                case 1: ThemeManager.SetDarkTheme(); break;
                case 2: ThemeManager.SetBlueTheme(); break;
                case 3: ThemeManager.SetPinkTheme(); break;
                case 4: ThemeManager.SetPurpleTheme(); break;
                case 5: ThemeManager.SetYellowTheme(); break;
                case 6: ThemeManager.SetRedTheme(); break;
                case 7: ThemeManager.SetBrownTheme(); break;
                default: ThemeManager.SetLightTheme(); break;
            }

            themePicker.SelectedIndex = index;
        }
    }

    // 🎨 APPLICA TEMA
    void ApplyTheme()
    {
        this.BackgroundColor = ThemeManager.BackgroundColor;

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