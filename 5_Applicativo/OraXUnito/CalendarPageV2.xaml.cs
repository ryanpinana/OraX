using System.Collections.ObjectModel;
using System.ComponentModel;
using Plugin.Maui.Calendar.Models;
using OraX.Services;
using OraX.Models;

namespace OraX;

public partial class CalendarPageV2 : ContentPage, INotifyPropertyChanged
{
    readonly DatabaseService database;
    List<Calendario> calendari = new();
    Calendario? calendarioCorrente;
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
    public EventCollection EventiCalendario { get; set; } = new();
    public List<Tipo> tuttiTipi { get; set; } = new();

    bool nessunRisultato;
    public bool NessunRisultato
    {
        get => nessunRisultato;
        set { nessunRisultato = value; OnPropertyChanged(); }
    }

    List<Attivita> tutteLeAttivita = new();
    Attivita? attivitaInModifica;
    Attivita? attivitaDettaglioCorrente;

    public CalendarPageV2(DatabaseService db)
    {
        database = db;
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadTheme();
        await CaricaTipi();
        await CaricaCalendari();

        var user = UserSession.CurrentUser;
        if (user != null)
            LabelNomeUtente.Text = $"{user.Nome} {user.Cognome}";
    }

    async Task CaricaTipi()
    {
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

    async Task CaricaCalendari()
    {
        var user = UserSession.CurrentUser;
        if (user == null) return;

        calendari = await database.GetCalendariUtente(user.Username);

        if (!calendari.Any())
        {
            var calendario = new Calendario
            {
                Nome = "Il mio calendario",
                CreatoreUsername = user.Username,
                Condiviso = false
            };

            int calendarioId = await database.SalvaCalendario(calendario);
            await database.AggiungiUtenteCalendario(new CalendarioUtente
            {
                CalendarioId = calendarioId,
                Username = user.Username
            });

            calendari = await database.GetCalendariUtente(user.Username);
        }

        CalendarioPicker.SelectedItem = null;
        CalendarioPicker.ItemsSource = null;
        CalendarioPicker.ItemsSource = calendari;
        CalendarioPicker.ItemDisplayBinding = new Binding(nameof(Calendario.Nome));

        int ultimoId = Preferences.Get("CalendarioCorrente", 0);
        calendarioCorrente = calendari.FirstOrDefault(c => c.Id == ultimoId) ?? calendari.FirstOrDefault();
        CalendarioPicker.SelectedItem = calendarioCorrente;

        if (calendarioCorrente != null)
        {
            Preferences.Set("CalendarioCorrente", calendarioCorrente.Id);
            await CaricaAttivitaCalendario();
        }
    }

    async Task CaricaAttivitaCalendario()
    {
        if (calendarioCorrente == null) return;

        var attivitaDb = await database.GetAttivitaCalendario(calendarioCorrente.Id);

        tutteLeAttivita.Clear();
        EventiCalendario = new EventCollection();

        foreach (var aDb in attivitaDb)
        {
            Tipo? tipo = aDb.TipoId.HasValue
                ? tuttiTipi.FirstOrDefault(t => t.Id == aDb.TipoId.Value)
                : null;

            var attivita = new Attivita
            {
                Id = aDb.Id,
                Titolo = aDb.Titolo,
                Data = aDb.Data,
                DataFine = aDb.DataFine,
                Colore = Color.FromArgb(string.IsNullOrWhiteSpace(aDb.ColoreHex) ? "#512BD4" : aDb.ColoreHex),
                Tipo = tipo,
                Note = aDb.Note,
                Username = aDb.Username,
                CalendarioId = aDb.CalendarioId,
                NotificheAttive = aDb.NotificheAttive,
                MinutiPreavviso = aDb.MinutiPreavviso,
                Completata = aDb.Completata
            };

            tutteLeAttivita.Add(attivita);
            AggiungiACalendario(attivita);
        }

        OnPropertyChanged(nameof(EventiCalendario));
        AggiornaAttivita(DataSelezionata);
    }

    void AggiungiACalendario(Attivita a)
    {
        DateTime fine = a.DataFine ?? a.Data;

        for (DateTime d = a.Data.Date; d <= fine.Date; d = d.AddDays(1))
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

        OnPropertyChanged(nameof(EventiCalendario));
    }

    void RimuoviDaCalendario(Attivita a)
    {
        DateTime fine = a.DataFine ?? a.Data;

        for (DateTime d = a.Data.Date; d <= fine.Date; d = d.AddDays(1))
        {
            if (!EventiCalendario.ContainsKey(d)) continue;

            var lista = EventiCalendario[d]
                .Cast<object>()
                .Where(x => x is not Attivita att || att.Id != a.Id)
                .ToList();

            if (lista.Count == 0)
                EventiCalendario.Remove(d);
            else
                EventiCalendario[d] = lista;
        }

        OnPropertyChanged(nameof(EventiCalendario));
    }

    void RicostruisciCalendario()
    {
        // Il controllo Calendar non sempre si aggiorna se modifichiamo
        // solamente le liste interne dell'EventCollection.
        // Per questo ricreo tutta la collezione e notifico il binding.
        var nuoviEventi = new EventCollection();

        foreach (var a in tutteLeAttivita)
        {
            DateTime fine = a.DataFine ?? a.Data;

            for (DateTime d = a.Data.Date; d <= fine.Date; d = d.AddDays(1))
            {
                if (nuoviEventi.ContainsKey(d))
                {
                    var lista = nuoviEventi[d].Cast<object>().ToList();
                    lista.Add(a);
                    nuoviEventi[d] = lista;
                }
                else
                {
                    nuoviEventi.Add(d, new List<object> { a });
                }
            }
        }

        EventiCalendario = nuoviEventi;
        OnPropertyChanged(nameof(EventiCalendario));
    }

    void AggiornaAttivita(DateTime giorno)
    {
        AttivitaDelGiorno.Clear();

        foreach (var a in tutteLeAttivita.OrderBy(a => a.Data))
        {
            DateTime fine = a.DataFine ?? a.Data;
            if (giorno.Date >= a.Data.Date && giorno.Date <= fine.Date)
                AttivitaDelGiorno.Add(a);
        }
    }

    void ApriOverlay(object sender, EventArgs e)
    {
        if (calendarioCorrente == null)
        {
            DisplayAlert("Errore", "Crea o seleziona prima un calendario", "OK");
            return;
        }

        attivitaInModifica = null;
        OverlayTitoloLabel.Text = "Nuova attività";
        SalvaAttivitaButton.Text = "Aggiungi";

        TitoloEntry.Text = "";
        NoteEditor.Text = "";
        ColorePicker.SelectedIndex = -1;
        TipoPicker.SelectedItem = null;
        DataInizioPicker.Date = DataSelezionata.Date;
        DataFinePicker.Date = DataSelezionata.Date;
        OraInizioPicker.Time = new TimeSpan(12, 0, 0);
        OraFinePicker.Time = new TimeSpan(13, 0, 0);
        DataFineSwitch.IsToggled = false;

        Overlay.IsVisible = true;
    }

    void ChiudiOverlay(object sender, EventArgs e)
    {
        attivitaInModifica = null;
        Overlay.IsVisible = false;
    }

    void ApriModificaAttivita(object sender, EventArgs e)
    {
        var attivita = (sender as Button)?.CommandParameter as Attivita;
        if (attivita == null) return;

        PreparaOverlayModifica(attivita);
    }

    void ModificaDaDettaglio(object sender, EventArgs e)
    {
        if (attivitaDettaglioCorrente == null) return;

        OverlayDettaglio.IsVisible = false;
        PreparaOverlayModifica(attivitaDettaglioCorrente);
    }

    void PreparaOverlayModifica(Attivita attivita)
    {
        attivitaInModifica = attivita;
        OverlayTitoloLabel.Text = "Modifica attività";
        SalvaAttivitaButton.Text = "Salva";

        TitoloEntry.Text = attivita.Titolo;
        NoteEditor.Text = attivita.Note;
        DataInizioPicker.Date = attivita.Data.Date;
        OraInizioPicker.Time = attivita.Data.TimeOfDay;

        DateTime fine = attivita.DataFine ?? attivita.Data;
        DataFinePicker.Date = fine.Date;
        OraFinePicker.Time = fine.TimeOfDay;
        DataFineSwitch.IsToggled = attivita.DataFine.HasValue && attivita.DataFine.Value.Date != attivita.Data.Date;

        TipoPicker.SelectedItem = attivita.Tipo;
        ColorePicker.SelectedIndex = IndiceColore(attivita.Colore);

        Overlay.IsVisible = true;
    }

    async void SalvaAttivitaClicked(object sender, EventArgs e)
    {
        if (attivitaInModifica == null)
            await AggiungiAttivita();
        else
            await ModificaAttivita();
    }

    async Task AggiungiAttivita()
    {

        if (calendarioCorrente == null)
        {
            await DisplayAlert("Errore", "Seleziona un calendario", "OK");
            return;
        }

        if (!ControllaCampi(out Tipo tipoSelezionato, out Color colore, out DateTime dataInizio, out DateTime? dataFine))
            return;

        var user = UserSession.CurrentUser;
        if (user == null) return;

        var attivitaDb = new AttivitaDb
        {
            Username = user.Username,
            Titolo = TitoloEntry.Text.Trim(),
            Data = dataInizio,
            DataFine = dataFine,
            ColoreHex = colore.ToArgbHex(),
            TipoId = tipoSelezionato.Id,
            Note = NoteEditor.Text?.Trim() ?? "",
            NotificheAttive = true,
            MinutiPreavviso = 0,
            CalendarioId = calendarioCorrente.Id,
            Completata = false,
            NotificaInviata = false
        };

        int newId = await database.SalvaAttivita(attivitaDb);

        var nuova = new Attivita
        {
            Id = newId,
            Titolo = attivitaDb.Titolo,
            Data = dataInizio,
            DataFine = dataFine,
            Colore = colore,
            Tipo = tipoSelezionato,
            Note = attivitaDb.Note,
            Username = user.Username,
            CalendarioId = calendarioCorrente.Id,
            Completata = false
        };

        tutteLeAttivita.Add(nuova);
        RicostruisciCalendario();
        AggiornaAttivita(DataSelezionata);

        Overlay.IsVisible = false;
        TitoloEntry.Text = "";
        NoteEditor.Text = "";
    }

    async Task ModificaAttivita()
    {
        if (attivitaInModifica == null) return;

        if (!ControllaCampi(out Tipo tipoSelezionato, out Color colore, out DateTime dataInizio, out DateTime? dataFine))
            return;

        attivitaInModifica.Titolo = TitoloEntry.Text.Trim();
        attivitaInModifica.Data = dataInizio;
        attivitaInModifica.DataFine = dataFine;
        attivitaInModifica.Colore = colore;
        attivitaInModifica.Tipo = tipoSelezionato;
        attivitaInModifica.Note = NoteEditor.Text?.Trim() ?? "";

        var attivitaDb = new AttivitaDb
        {
            Id = attivitaInModifica.Id,
            Username = attivitaInModifica.Username,
            Titolo = attivitaInModifica.Titolo,
            Data = attivitaInModifica.Data,
            DataFine = attivitaInModifica.DataFine,
            ColoreHex = colore.ToArgbHex(),
            TipoId = tipoSelezionato.Id,
            Note = attivitaInModifica.Note,
            NotificheAttive = attivitaInModifica.NotificheAttive,
            MinutiPreavviso = attivitaInModifica.MinutiPreavviso,
            CalendarioId = attivitaInModifica.CalendarioId,
            Completata = attivitaInModifica.Completata,
            NotificaInviata = false
        };

        int righeAggiornate = await database.AggiornaAttivita(attivitaDb);

        if (righeAggiornate == 0)
        {
            await DisplayAlert("Errore", "La modifica non è stata salvata nel database", "OK");
            return;
        }

        // Ricarico dal database: così la lista del giorno e il calendario
        // mostrano subito i dati aggiornati realmente salvati.
        await CaricaAttivitaCalendario();

        Overlay.IsVisible = false;
        attivitaInModifica = null;
    }

    bool ControllaCampi(out Tipo tipoSelezionato, out Color colore, out DateTime dataInizio, out DateTime? dataFine)
    {
        tipoSelezionato = null;
        colore = Colors.Transparent;
        dataInizio = DateTime.Now;
        dataFine = null;

        if (string.IsNullOrWhiteSpace(TitoloEntry.Text))
        {
            DisplayAlert("Errore", "Inserire un titolo per l'attività", "OK");
            return false;
        }

        tipoSelezionato = TipoPicker.SelectedItem as Tipo;
        if (tipoSelezionato == null)
        {
            DisplayAlert("Errore", "Scegliere un tipo per l'attività", "OK");
            return false;
        }

        colore = ColorePicker.SelectedIndex switch
        {
            0 => Colors.Red,
            1 => Colors.Green,
            2 => Colors.Blue,
            _ => tipoSelezionato.Colore
        };

        dataInizio = DataInizioPicker.Date + OraInizioPicker.Time;
        dataFine = DataFineSwitch.IsToggled
            ? DataFinePicker.Date + OraFinePicker.Time
            : dataInizio.Date + OraFinePicker.Time;

        if (dataFine.HasValue && dataFine.Value < dataInizio)
        {
            DisplayAlert("Errore", "La data di fine non può essere prima di quella d'inizio", "OK");
            return false;
        }

        return true;
    }

    int IndiceColore(Color colore)
    {
        string nome = NomeColore(colore);
        return nome switch
        {
            "Rosso" => 0,
            "Verde" => 1,
            "Blu" => 2,
            _ => -1
        };
    }

    void OnDataFineSwitchToggled(object sender, ToggledEventArgs e)
    {
        DataFinePicker.IsEnabled = e.Value;
        DataFinePicker.Opacity = e.Value ? 1 : 0.5;
    }

    async void EliminaAttivita(object sender, EventArgs e)
    {
        var attivita = (sender as Button)?.CommandParameter as Attivita;
        if (attivita == null) return;

        bool conferma = await DisplayAlert("Conferma", "Vuoi eliminare questa attività?", "Sì", "No");
        if (!conferma) return;

        await database.EliminaAttivita(attivita.Id);
        tutteLeAttivita.Remove(attivita);
        RicostruisciCalendario();
        AggiornaAttivita(DataSelezionata);
    }

    async void CompletaAttivita(object sender, EventArgs e)
    {
        var attivita = (sender as Button)?.CommandParameter as Attivita;
        if (attivita == null) return;

        attivita.Completata = !attivita.Completata;
        await database.ImpostaCompletata(attivita.Id, attivita.Completata);
        AggiornaAttivita(DataSelezionata);
    }

    void OnSearchPressed(object sender, EventArgs e)
    {
        string testo = SearchBarAttivita.Text?.ToLower().Trim() ?? "";
        RisultatiRicerca.Clear();

        var risultati = tutteLeAttivita
            .Where(a => string.IsNullOrWhiteSpace(testo) || a.Titolo.ToLower().Contains(testo))
            .OrderBy(a => a.Data)
            .ToList();

        foreach (var a in risultati)
            RisultatiRicerca.Add(a);

        NessunRisultato = risultati.Count == 0;
        SearchOverlay.IsVisible = true;
    }

    void ChiudiSearchOverlay(object sender, EventArgs e) => SearchOverlay.IsVisible = false;

    void OnRisultatoSelezionato(object sender, SelectionChangedEventArgs e)
    {
        var attivita = e.CurrentSelection.FirstOrDefault() as Attivita;
        if (attivita == null) return;

        DataSelezionata = attivita.Data.Date;
        SearchOverlay.IsVisible = false;
        (sender as CollectionView)!.SelectedItem = null;
    }

    void OnAttivitaSelezionata(object sender, SelectionChangedEventArgs e)
    {
        var attivita = e.CurrentSelection.FirstOrDefault() as Attivita;
        if (attivita == null) return;

        attivitaDettaglioCorrente = attivita;
        DettaglioHeader.BackgroundColor = attivita.Colore;
        DettaglioTitolo.Text = attivita.Titolo;
        DettaglioDataInizio.Text = attivita.Data.ToString("dd MMMM yyyy HH:mm");
        DettaglioDataFine.Text = attivita.DataFine.HasValue
            ? attivita.DataFine.Value.ToString("dd MMMM yyyy HH:mm")
            : "Nessuna";

        DettaglioColore.Color = attivita.Colore;
        DettaglioColoreNome.Text = NomeColore(attivita.Colore);

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

        DettaglioNote.Text = string.IsNullOrWhiteSpace(attivita.Note) ? "Nessuna nota" : attivita.Note;
        OverlayDettaglio.IsVisible = true;
        (sender as CollectionView)!.SelectedItem = null;
    }

    void ChiudiOverlayDettaglio(object sender, EventArgs e) => OverlayDettaglio.IsVisible = false;

    async void CreaCalendarioClicked(object sender, EventArgs e)
    {
        string nome = await DisplayPromptAsync("Nuovo calendario", "Nome calendario");
        if (string.IsNullOrWhiteSpace(nome)) return;

        var user = UserSession.CurrentUser;
        if (user == null) return;

        var calendario = new Calendario
        {
            Nome = nome.Trim(),
            CreatoreUsername = user.Username,
            Condiviso = false
        };

        int calendarioId = await database.SalvaCalendario(calendario);
        await database.AggiungiUtenteCalendario(new CalendarioUtente
        {
            CalendarioId = calendarioId,
            Username = user.Username
        });

        Preferences.Set("CalendarioCorrente", calendarioId);
        await CaricaCalendari();
        await DisplayAlert("Successo", "Calendario creato", "OK");
    }

    async void CalendarioCambiato(object sender, EventArgs e)
    {
        calendarioCorrente = CalendarioPicker.SelectedItem as Calendario;
        if (calendarioCorrente == null) return;

        Preferences.Set("CalendarioCorrente", calendarioCorrente.Id);
        await CaricaAttivitaCalendario();
    }

    async void InvitaClicked(object sender, EventArgs e)
    {
        if (calendarioCorrente == null) return;

        string username = await DisplayPromptAsync("Invita utente", "Username");
        if (string.IsNullOrWhiteSpace(username)) return;

        username = username.Trim();
        var user = UserSession.CurrentUser;
        if (user == null) return;

        if (username.Equals(user.Username, StringComparison.OrdinalIgnoreCase))
        {
            await DisplayAlert("Errore", "Non puoi invitare te stesso", "OK");
            return;
        }

        var utente = await database.GetUserByUsername(username);
        if (utente == null)
        {
            await DisplayAlert("Errore", "Utente inesistente", "OK");
            return;
        }

        username = utente.Username;

        bool giaDentro = await database.UtenteGiaNelCalendario(calendarioCorrente.Id, username);
        if (giaDentro)
        {
            await DisplayAlert("Info", "Questo utente è già nel calendario", "OK");
            return;
        }

        bool richiestaGiaInviata = await database.RichiestaPendenteEsiste(calendarioCorrente.Id, username);
        if (richiestaGiaInviata)
        {
            await DisplayAlert("Info", "Hai già inviato una richiesta a questo utente", "OK");
            return;
        }

        await database.InviaRichiesta(new RichiestaCondivisione
        {
            MittenteUsername = user.Username,
            DestinatarioUsername = username,
            CalendarioId = calendarioCorrente.Id,
            Stato = "In attesa"
        });

        calendarioCorrente.Condiviso = true;
        await database.AggiornaCalendario(calendarioCorrente);

        await DisplayAlert("Successo", "Invito inviato", "OK");
    }

    async void MostraRichiesteClicked(object sender, EventArgs e)
    {
        var user = UserSession.CurrentUser;
        if (user == null) return;

        var richieste = await database.GetRichieste(user.Username);
        if (richieste.Count == 0)
        {
            await DisplayAlert("Richieste", "Nessuna richiesta pendente", "OK");
            return;
        }

        foreach (var richiesta in richieste)
        {
            var calendario = await database.GetCalendarioById(richiesta.CalendarioId);
            string risposta = await DisplayActionSheet(
                $"{richiesta.MittenteUsername} ti ha invitato nel calendario '{calendario?.Nome}'",
                "Chiudi",
                null,
                "Accetta",
                "Rifiuta");

            if (risposta == "Accetta")
            {
                bool giaDentro = await database.UtenteGiaNelCalendario(richiesta.CalendarioId, user.Username);
                if (!giaDentro)
                {
                    await database.AggiungiUtenteCalendario(new CalendarioUtente
                    {
                        CalendarioId = richiesta.CalendarioId,
                        Username = user.Username
                    });
                }

                richiesta.Stato = "Accettata";
                await database.AggiornaRichiesta(richiesta);
            }
            else if (risposta == "Rifiuta")
            {
                richiesta.Stato = "Rifiutata";
                await database.AggiornaRichiesta(richiesta);
            }
        }

        await CaricaCalendari();
    }

    string NomeColore(Color colore)
    {
        string hex = colore.ToArgbHex().ToUpper();

        if (hex.EndsWith("FF0000")) return "Rosso";
        if (hex.EndsWith("008000") || hex.EndsWith("00FF00")) return "Verde";
        if (hex.EndsWith("0000FF")) return "Blu";

        var tipo = tuttiTipi.FirstOrDefault(t => t.Colore.ToArgbHex().ToUpper() == hex);
        if (tipo != null) return tipo.Nome;

        return hex;
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
        BackgroundColor = ThemeManager.BackgroundColor;
        LabelNomeUtente.TextColor = ThemeManager.TextColor;
    }
}
