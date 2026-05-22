using OraX.Models;
using OraX.Services;

namespace OraX;

public partial class Statistiche : ContentPage
{
    User user;
    DatabaseService database = new();

    public Statistiche()
    {
        InitializeComponent();
        user = UserSession.CurrentUser;
    }
    async Task CaricaStatistiche()
    {
        var calendarioCorrente =
            Preferences.Get("CalendarioCorrente", 0);

        var attivita =
            await database.GetAttivitaUtenteCalendario(
                calendarioCorrente,
                user.Username);

        int completate =
            attivita.Count(a => a.Completata);

        int nonCompletate =
            attivita.Count(a => !a.Completata);

        int totale =
            attivita.Count;

        double percentuale = 0;

        if (totale > 0)
        {
            percentuale =
                (double)completate / totale * 100;
        }

        // LABEL PRINCIPALI
        labelFatte.Text =
            completate.ToString();

        labelNonFatte.Text =
            nonCompletate.ToString();

        labelTot.Text =
            $"Totale: {totale} attività";

        labelPercento.Text =
            $"{Math.Round(percentuale)}% completato";

        progressBar.Progress = percentuale / 100;

        // OGGI
        var oggi =
            attivita.Where(a =>
                a.Data.Date ==
                DateTime.Today);

        int oggiComplete =
            oggi.Count(a => a.Completata);

        int oggiNonComplete =
            oggi.Count(a => !a.Completata);

        labelQuante.Text =
            $"{oggiComplete} completate - {oggiNonComplete} non completate";

        // IERI
        var ieri =
            attivita.Where(a =>
                a.Data.Date ==
                DateTime.Today.AddDays(-1));

        labelStatistiche.Text =
            $"{ieri.Count(a => a.Completata)} completate - " +
            $"{ieri.Count(a => !a.Completata)} non completate";

        // 2 GIORNI FA
        var dueGiorni =
            attivita.Where(a =>
                a.Data.Date ==
                DateTime.Today.AddDays(-2));

        labelStati.Text =
            $"{dueGiorni.Count(a => a.Completata)} completate - " +
            $"{dueGiorni.Count(a => !a.Completata)} non completate";
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        user = UserSession.CurrentUser;
        

        LoadTheme();
        await CaricaStatistiche();
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