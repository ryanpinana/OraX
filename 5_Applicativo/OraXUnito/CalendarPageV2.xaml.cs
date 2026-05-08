using System.Collections.ObjectModel;
using System.ComponentModel;
using Plugin.Maui.Calendar.Models;
using OraX.Services;
using OraX.Models;

namespace OraX;

public partial class CalendarPageV2 : ContentPage, INotifyPropertyChanged
{
    readonly DatabaseService database;

    DateTime dataSelezionata = DateTime.Today;

    public DateTime DataSelezionata
    {
        get => dataSelezionata;
        set
        {
            if (dataSelezionata != value)
            {
                dataSelezionata = value;
                OnPropertyChanged();
                AggiornaAttivita(value);
            }
        }
    }

    public ObservableCollection<Attivita> AttivitaDelGiorno { get; set; } = new();
    public ObservableCollection<Attivita> RisultatiRicerca { get; set; } = new();

    bool nessunRisultato;
    public bool NessunRisultato
    {
        get => nessunRisultato;
        set { nessunRisultato = value; OnPropertyChanged(); }
    }

    public EventCollection EventiCalendario { get; set; } = new();

    List<Attivita> tutteLeAttivita = new();
    public List<Tipo> tuttiTipi { get; set; } = new();

    // -------------------------
    // Costruttore
    // -------------------------

    public CalendarPageV2(DatabaseService db)
    {
        database = db;

        InitializeComponent();
        BindingContext = this;
    }

    // Carica dati dal DB quando la pagina appare
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CaricaTipi();
        await CaricaAttivita();

        // Aggiorna nome utente nell'header
        var user = UserSession.CurrentUser;
        if (user != null)
        {
            LabelNomeUtente.Text = $"{user.Nome} {user.Cognome}";
            ImageUtente.Source = user.FotoProfiloPath;
        }
    }

    // -------------------------
    // Caricamento da DB
    // -------------------------

    async Task CaricaTipi()
    {
        // Crea i tipi default se è il primo avvio
        await database.InitTipiDefault();

        var tipiDb = await database.GetTipi();

        tuttiTipi = tipiDb.Select(t => new Tipo
        {
            Id = t.Id,
            Nome = t.Nome,
            Colore = Color.FromArgb(t.ColoreHex)
        }).ToList();

        OnPropertyChanged(nameof(tuttiTipi));
    }

    async Task CaricaAttivita()
    {
        var user = UserSession.CurrentUser;
        if (user == null) return;

        var attivitaDb = await database.GetAttivitaByUser(user.Username);

        tutteLeAttivita.Clear();
        EventiCalendario.Clear();

        foreach (var aDb in attivitaDb)
        {
            // Ricostruisce il tipo dall'id
            Tipo? tipo = aDb.TipoId.HasValue
                ? tuttiTipi.FirstOrDefault(t => t.Id == aDb.TipoId.Value)
                : null;

            var attivita = new Attivita
            {
                Id = aDb.Id,
                Titolo = aDb.Titolo,
                Data = aDb.Data,
                DataFine = aDb.DataFine,
                Colore = Color.FromArgb(aDb.ColoreHex),
                Tipo = tipo,
                Note = aDb.Note
            };

            tutteLeAttivita.Add(attivita);
            AggiungiACalendario(attivita);
        }

        AggiornaAttivita(DataSelezionata);
    }

    // -------------------------
    // Logica calendario
    // -------------------------

    void AggiungiACalendario(Attivita a)
    {
        DateTime fine = a.DataFine ?? a.Data;

        for (DateTime d = a.Data; d <= fine; d = d.AddDays(1))
        {
            if (EventiCalendario.ContainsKey(d))
            {
                var lista = EventiCalendario[d].Cast<object>().ToList();
                lista.Add(a);
                EventiCalendario[d] = lista;
            }
            else
            {
                EventiCalendario.Add(d, new List<object> { a });
            }
        }
    }

    void RimuoviDaCalendario(Attivita a)
    {
        DateTime fine = a.DataFine ?? a.Data;

        for (DateTime d = a.Data; d <= fine; d = d.AddDays(1))
        {
            if (!EventiCalendario.ContainsKey(d))
                continue;

            var lista = EventiCalendario[d].Cast<object>().ToList();
            lista.Remove(a);

            if (lista.Count == 0)
                EventiCalendario.Remove(d);
            else
                EventiCalendario[d] = lista;
        }
    }

    void AggiornaAttivita(DateTime giorno)
    {
        AttivitaDelGiorno.Clear();

        foreach (var a in tutteLeAttivita)
        {
            DateTime fine = a.DataFine ?? a.Data;
            if (giorno.Date >= a.Data.Date && giorno.Date <= fine.Date)
                AttivitaDelGiorno.Add(a);
        }
    }

    // -------------------------
    // Overlay aggiunta attività
    // -------------------------

    void ApriOverlay(object sender, EventArgs e)
    {
        Overlay.IsVisible = true;
    }

    void ChiudiOverlay(object sender, EventArgs e)
    {
        Overlay.IsVisible = false;
    }

    async void AggiungiAttivita(object sender, EventArgs e)
    {
        // Validazione titolo
        if (string.IsNullOrWhiteSpace(TitoloEntry.Text))
        {
            await DisplayAlert("Errore", "Inserire un titolo per l'attività", "OK");
            return;
        }

        // Validazione tipo
        var tipoSelezionato = TipoPicker.SelectedItem as Tipo;
        if (tipoSelezionato == null)
        {
            await DisplayAlert("Errore", "Scegliere un tipo per l'attività", "OK");
            return;
        }

        // Lettura colore
        Color colore = ColorePicker.SelectedIndex switch
        {
            0 => Colors.Red,
            1 => Colors.Green,
            2 => Colors.Blue,
            _ => Colors.Red
        };

        // Lettura data fine
        DateTime dataInizio = DataSelezionata;
        DateTime? dataFine = DataFineSwitch.IsToggled ? DataFinePicker.Date : null;

        // Validazione data fine
        if (dataFine.HasValue && dataFine.Value < dataInizio)
        {
            await DisplayAlert("Errore", "La data di fine non può essere prima di quella d'inizio", "OK");
            return;
        }

        var user = UserSession.CurrentUser;

        // Salva nel DB
        var attivitaDb = new AttivitaDb
        {
            Username = user.Username,
            Titolo = TitoloEntry.Text.Trim(),
            Data = dataInizio,
            DataFine = dataFine,
            ColoreHex = colore.ToArgbHex(),
            TipoId = tipoSelezionato.Id,
            Note = NoteEditor.Text?.Trim() ?? ""
        };

        int newId = await database.SalvaAttivita(attivitaDb);

        // Crea l'oggetto in memoria con l'id assegnato dal DB
        var nuova = new Attivita
        {
            Id = newId,
            Titolo = attivitaDb.Titolo,
            Data = dataInizio,
            DataFine = dataFine,
            Colore = colore,
            Tipo = tipoSelezionato,
            Note = attivitaDb.Note
        };

        tutteLeAttivita.Add(nuova);
        AggiungiACalendario(nuova);
        AggiornaAttivita(DataSelezionata);

        Overlay.IsVisible = false;
        TitoloEntry.Text = "";
    }

    void OnDataFineSwitchToggled(object sender, ToggledEventArgs e)
    {
        DataFinePicker.IsEnabled = e.Value;
        DataFinePicker.Opacity = e.Value ? 1 : 0.5;
    }

    // -------------------------
    // Eliminazione attività
    // -------------------------

    async void EliminaAttivita(object sender, EventArgs e)
    {
        var attivita = (sender as Button)?.CommandParameter as Attivita;

        if (attivita == null)
            return;

        bool conferma = await DisplayAlert("Conferma", "Vuoi eliminare questa attività?", "Sì", "No");

        if (!conferma)
            return;

        // Rimuove dal DB
        await database.EliminaAttivita(attivita.Id);

        tutteLeAttivita.Remove(attivita);
        RimuoviDaCalendario(attivita);
        AggiornaAttivita(DataSelezionata);
    }

    // -------------------------
    // Ricerca
    // -------------------------

    void OnSearchPressed(object sender, EventArgs e)
    {
        string testo = SearchBarAttivita.Text?.ToLower().Trim() ?? "";

        RisultatiRicerca.Clear();

        var risultati = tutteLeAttivita
            .Where(a => a.Titolo.ToLower().Contains(testo))
            .ToList();

        foreach (var a in risultati)
            RisultatiRicerca.Add(a);

        NessunRisultato = risultati.Count == 0;
        SearchOverlay.IsVisible = true;
    }

    void ChiudiSearchOverlay(object sender, EventArgs e)
    {
        SearchOverlay.IsVisible = false;
    }

    void OnRisultatoSelezionato(object sender, SelectionChangedEventArgs e)
    {
        var attivita = e.CurrentSelection.FirstOrDefault() as Attivita;

        if (attivita == null)
            return;

        DataSelezionata = attivita.Data;
        SearchOverlay.IsVisible = false;
        (sender as CollectionView).SelectedItem = null;
    }

    // -------------------------
    // Dettaglio attività
    // -------------------------

    void OnAttivitaSelezionata(object sender, SelectionChangedEventArgs e)
    {
        var attivita = e.CurrentSelection.FirstOrDefault() as Attivita;

        if (attivita == null)
            return;

        DettaglioHeader.BackgroundColor = attivita.Colore;
        DettaglioTitolo.Text = attivita.Titolo;
        DettaglioDataInizio.Text = attivita.Data.ToString("dd MMMM yyyy");
        DettaglioDataFine.Text = attivita.DataFine.HasValue
            ? attivita.DataFine.Value.ToString("dd MMMM yyyy")
            : "Nessuna";

        DettaglioColore.Color = attivita.Colore;
        DettaglioColoreNome.Text = attivita.Colore == Colors.Red   ? "Rosso"
                                 : attivita.Colore == Colors.Green ? "Verde"
                                 : attivita.Colore == Colors.Blue  ? "Blu"
                                 : attivita.Colore.ToString();

        if (attivita.Tipo != null)
        {
            DettaglioTipoTag.BackgroundColor = attivita.Tipo.Colore;
            DettaglioTipoNome.Text = attivita.Tipo.Nome;
            DettaglioTipoTag.IsVisible = true;
        }
        else
        {
            DettaglioTipoTag.IsVisible = false;
        }

        DettaglioNote.Text = attivita.Note;
        OverlayDettaglio.IsVisible = true;

        (sender as CollectionView).SelectedItem = null;
    }

    void ChiudiOverlayDettaglio(object sender, EventArgs e)
    {
        OverlayDettaglio.IsVisible = false;
    }
}
