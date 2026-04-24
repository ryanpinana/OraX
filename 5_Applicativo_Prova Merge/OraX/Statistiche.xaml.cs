using OraX.Models;
using OraX.Services;

namespace OraX;

public partial class Statistiche : ContentPage
{
    User user;

    public Statistiche()
    {
        InitializeComponent();
        user = UserSession.CurrentUser;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        user = UserSession.CurrentUser;


        LoadTheme();
    }

    void LoadTheme()
    {
        int index = Preferences.Get("AppTheme", 0);

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