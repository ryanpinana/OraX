using Microsoft.Maui.ApplicationModel;

namespace OraX;

public partial class Statistiche : ContentPage
{
	public Statistiche()
	{
		InitializeComponent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadTheme();
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
        this.BackgroundColor = ThemeManager.BackgroundColor;

        labelHeader.TextColor = ThemeManager.TextColor;
        labelCompletamento.TextColor = ThemeManager.TextColor;
        labelNonComplete.TextColor = ThemeManager.TextColor;
        labelFatte.TextColor = ThemeManager.TextColor;
        labelNonFatte.TextColor = ThemeManager.TextColor;
        labelTot.TextColor = ThemeManager.TextColor;
        labelComplete.TextColor = ThemeManager.TextColor;
        labelPercento.TextColor = ThemeManager.TextColor;
        labelAttivita.TextColor = ThemeManager.TextColor;
        labelOggi.TextColor = ThemeManager.TextColor;
        labelQuante.TextColor = ThemeManager.TextColor;
        labelIeri.TextColor = ThemeManager.TextColor;
        labelStatistiche.TextColor = ThemeManager.TextColor;
        label2D.TextColor = ThemeManager.TextColor;
        labelStati.TextColor = ThemeManager.TextColor;


        frameSummary.BackgroundColor = ThemeManager.FrameBackgroundColor;
        frameSummary.BorderColor = ThemeManager.FrameBorderColor;
        frameCard.BackgroundColor = ThemeManager.FrameBackgroundColor;
        frameCard.BorderColor = ThemeManager.FrameBorderColor;
        frameItem.BackgroundColor = ThemeManager.FrameBackgroundColor;
        frameItem.BorderColor = ThemeManager.FrameBorderColor;
        frameAttivita.BackgroundColor = ThemeManager.FrameBackgroundColor;
        frameAttivita.BorderColor = ThemeManager.FrameBorderColor;
        frameAttivita2.BackgroundColor = ThemeManager.FrameBackgroundColor;
        frameAttivita2.BorderColor = ThemeManager.FrameBorderColor;
    }
}