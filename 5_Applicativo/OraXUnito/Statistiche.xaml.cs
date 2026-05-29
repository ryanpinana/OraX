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
        user = UserSession.CurrentUser;

        if (user == null)
        {
            labelFatte.Text = "0";
            labelNonFatte.Text = "0";
            labelTot.Text = "Totale: 0 attività";
            labelPercento.Text = "0% completato";
            progressBar.Progress = 0;
            labelQuante.Text = "0 completate · 0 non completate";
            labelStatistiche.Text = "0 completate · 0 non completate";
            labelStati.Text = "0 completate · 0 non completate";
            return;
        }

        int calendarioCorrente = Preferences.Get("CalendarioCorrente", 0);

        var attivita = calendarioCorrente > 0
            ? await database.GetAttivitaUtenteCalendario(calendarioCorrente, user.Username)
            : await database.GetAttivitaUtente(user.Username);

        int completate = attivita.Count(a => a.Completata);
        int nonCompletate = attivita.Count(a => !a.Completata);
        int totale = attivita.Count;

        double percentuale = totale > 0
            ? (double)completate / totale * 100
            : 0;

        labelFatte.Text = completate.ToString();
        labelNonFatte.Text = nonCompletate.ToString();
        labelTot.Text = $"Totale: {totale} attività";
        labelPercento.Text = $"{Math.Round(percentuale)}% completato";
        progressBar.Progress = percentuale / 100;

        AggiornaRigaGiorno(attivita, DateTime.Today, labelQuante);
        AggiornaRigaGiorno(attivita, DateTime.Today.AddDays(-1), labelStatistiche);
        AggiornaRigaGiorno(attivita, DateTime.Today.AddDays(-2), labelStati);
    }

    void AggiornaRigaGiorno(IEnumerable<AttivitaDb> attivita, DateTime giorno, Label label)
    {
        var delGiorno = attivita.Where(a => a.Data.Date == giorno.Date);
        int complete = delGiorno.Count(a => a.Completata);
        int nonComplete = delGiorno.Count(a => !a.Completata);

        label.Text = $"{complete} completate · {nonComplete} non completate";
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
