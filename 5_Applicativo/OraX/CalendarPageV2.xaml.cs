using System.Collections.ObjectModel;
using System.ComponentModel;
using Plugin.Maui.Calendar.Models;

namespace OraX;

public partial class CalendarPageV2 : ContentPage, INotifyPropertyChanged
{
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
        set
        {
            nessunRisultato = value;
            OnPropertyChanged();
        }
    }

    public EventCollection EventiCalendario { get; set; } = new();

    List<Attivita> tutteLeAttivita = new();
    public List<Tipo> tuttiTipi { get; set; } = new();

    // Costruttore

    public CalendarPageV2()
    {
        tuttiTipi = new List<Tipo>
        {
            new Tipo { Nome = "Scuola", Colore = Colors.Aquamarine },
            new Tipo { Nome = "Casa",   Colore = Colors.DarkCyan   },
            new Tipo { Nome = "Viaggi", Colore = Colors.Gold       },
        };

        tutteLeAttivita = new List<Attivita>
        {
            new Attivita { Titolo = "Verifica M145",  Data = new DateTime(2026, 4, 15), Colore = Colors.Red,   Tipo = tuttiTipi[0] },
            new Attivita { Titolo = "Minecraft Live", Data = new DateTime(2026, 4, 15), Colore = Colors.Green, Tipo = tuttiTipi[1] },
            new Attivita { Titolo = "Cinema con Luca",Data = new DateTime(2026, 4, 20), Colore = Colors.Red,   Tipo = tuttiTipi[1] },
        };

        InitializeComponent();
        BindingContext = this;

        foreach (var a in tutteLeAttivita)
        {
            AggiungiACalendario(a);
        }

        AggiornaAttivita(DateTime.Today);
    }

    // Logica calendario

    // Aggiunge un'attività a EventiCalendario per ogni giorno del suo range
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

    // Rimuove un'attività da EventiCalendario per ogni giorno del suo range
    void RimuoviDaCalendario(Attivita a)
    {
        DateTime fine = a.DataFine ?? a.Data;

        for (DateTime d = a.Data; d <= fine; d = d.AddDays(1))
        {
            if (!EventiCalendario.ContainsKey(d))
            {
                continue;
            }

            var lista = EventiCalendario[d].Cast<object>().ToList();
            lista.Remove(a);

            if (lista.Count == 0)
                EventiCalendario.Remove(d);
            else
                EventiCalendario[d] = lista;
        }
    }

    // Mostra solo le attività che coprono il giorno selezionato
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

    // Overlay aggiunta attività

    void ApriOverlay(object sender, EventArgs e)
    {
        Overlay.IsVisible = true;
    }

    void ChiudiOverlay(object sender, EventArgs e)
    {
        Overlay.IsVisible = false;
    }

    void AggiungiAttivita(object sender, EventArgs e)
    {
        // Validazione titolo
        if (string.IsNullOrWhiteSpace(TitoloEntry.Text))
        {
            DisplayAlert("Errore", "Inserire un titolo per l'attività", "OK");
            return;
        }

        // Validazione tipo
        var tipoSelezionato = TipoPicker.SelectedItem as Tipo;
        if (tipoSelezionato == null)
        {
            DisplayAlert("Errore", "Scegliere un tipo per l'attività", "OK");
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
            DisplayAlert("Errore", "La data di fine non può essere prima di quella d'inizio", "OK");
            return;
        }

        var nuova = new Attivita
        {
            Titolo = TitoloEntry.Text.Trim(),
            Data = dataInizio,
            DataFine = dataFine,
            Colore = colore,
            Tipo = tipoSelezionato,
            Note = NoteEditor.Text?.Trim() ?? ""
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

    // Eliminazione attività

    async void EliminaAttivita(object sender, EventArgs e)
    {
        var attivita = (sender as Button)?.CommandParameter as Attivita;

        if (attivita == null)
            return;

        bool conferma = await DisplayAlert("Conferma", "Vuoi eliminare questa attività?", "Sì", "No");

        if (!conferma)
            return;

        tutteLeAttivita.Remove(attivita);
        RimuoviDaCalendario(attivita);
        AggiornaAttivita(DataSelezionata);
    }

    // Ricerca

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

    // Dettaglio attività

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

        DettaglioColoreNome.Text = attivita.Colore == Colors.Red ? "Rosso"
                                 : attivita.Colore == Colors.Green ? "Verde"
                                 : attivita.Colore == Colors.Blue ? "Blu"
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