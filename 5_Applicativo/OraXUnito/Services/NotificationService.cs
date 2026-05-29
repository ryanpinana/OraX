using OraX.Models;

namespace OraX.Services;

public class NotificationService
{
    private readonly DatabaseService database;
    private IDispatcherTimer? timer;
    private DateTime ultimoControllo = DateTime.Now.AddSeconds(-30);

    public NotificationService(DatabaseService database)
    {
        this.database = database;
    }

    public void Avvia()
    {
        if (timer != null)
            return;

        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(30);

        // controllo ogni 30 secondi, così non devo usare librerie esterne
        timer.Tick += async (sender, e) =>
        {
            await ControllaNotifiche();
        };

        timer.Start();
    }

    private async Task ControllaNotifiche()
    {
        DateTime adesso = DateTime.Now;
        List<AttivitaDb> attivita = await database.GetAttivitaPerNotifiche();

        foreach (AttivitaDb item in attivita)
        {
            DateTime scadenza = item.DataFine ?? item.Data;

            bool deveNotificare =
                scadenza > ultimoControllo &&
                scadenza <= adesso;

            if (!deveNotificare)
                continue;

            Page? pagina = Application.Current?.Windows.FirstOrDefault()?.Page
                           ?? Application.Current?.MainPage;

            if (pagina != null)
            {
                await pagina.DisplayAlert(
                    "Promemoria",
                    $"L'attività \"{item.Titolo}\" scade adesso.",
                    "OK"
                );
            }

            // così la stessa attività non continua a notificare ogni volta
            await database.SegnaNotificaInviata(item.Id);
        }

        ultimoControllo = adesso;
    }
}
