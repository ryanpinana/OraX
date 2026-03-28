using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Plugin.Maui.Calendar.Models;
using System.Globalization;

namespace OraX;

public partial class CalendarPageV2 : ContentPage, INotifyPropertyChanged
{
	DateTime dataSelezionata = DateTime.Today;

	public DateTime DataSelezionata
	{
		get => dataSelezionata;

		set
		{
			if(dataSelezionata != value)
			{
				dataSelezionata = value;
				OnPropertyChanged();  //Notifica la UI

				AggiornaAttivita(value);  //Aggiorna la lista degli eventi
			}
		}
	}

	//Binding con la UI per mostrare le attivita (OC notifica automaticamente la UI)
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

	//Binding col calendario per i puntini
	public EventCollection EventiCalendario { get; set; } = new();

	//Sorgente di tutti i dati "privati" (provissorio)
	List<Attivita> tutteLeAttivita = new();
	public CalendarPageV2()
	{
		InitializeComponent();

		BindingContext = this;

		tutteLeAttivita = new List<Attivita>
		{
			new Attivita
			{
				Titolo = "Verifica M145",
				Data = new DateTime(2026, 3, 15),
				Colore = Colors.Red
			},
			new Attivita
			{
				Titolo = "Minecraft Live",
				Data = new DateTime(2026, 3, 15),
				Colore = Colors.Green
			},
			new Attivita
			{
				Titolo = "Cinema con Luca",
				Data = new DateTime(2026, 3, 20),
				Colore = Colors.Red
			}
		};

		foreach (var a in tutteLeAttivita)
		{
			if(EventiCalendario.ContainsKey(a.Data))
			{
				var lista = EventiCalendario[a.Data].Cast<object>().ToList();
				lista.Add(a);
				EventiCalendario[a.Data] = lista;
			}else
			{
				EventiCalendario.Add(a.Data, new List<object> { a });
			}
		}

		AggiornaAttivita(DateTime.Today);
	}

	//filtra le attività e mostra solo gli eventi del giorno scelto
	void AggiornaAttivita(DateTime giorno)
	{
		//Pulisce la lista attuale
		AttivitaDelGiorno.Clear();

		foreach (var a in tutteLeAttivita)
		{
			DateTime fine = a.DataFine ?? a.Data;  //Se non c'è dataFine, usa data

			if (giorno.Date >= a.Data.Date && giorno.Date <= fine.Date)
			{
				AttivitaDelGiorno.Add(a);
			}
		}

	}

	//Mostra il form
	void ApriOverlay(object sender, EventArgs e)
	{
		Overlay.IsVisible = true;
	}

	//Nasconde il form
	void ChiudiOverlay(object sender, EventArgs e)
	{
		Overlay.IsVisible = false;
	}

	//Aggiunta di un attività
	void AggiungiAttivita(object sender, EventArgs e)
	{

		Color colore = Colors.Red;

		//Legge dal form il colore scelto in base all'indice
		switch(ColorePicker.SelectedIndex)
		{
			case 0:
				colore = Colors.Red;
				break;

			case 1:
				colore = Colors.Green;
				break;

			case 2:
				colore = Colors.Blue;
				break;
		}

		DateTime dataInizio = DataSelezionata;
		DateTime? dataFine = null;

		//Se lo switch per la datafine è attivo, legge il valore messo
		if(DataFineSwitch.IsToggled)
		{
			dataFine = DataFinePicker.Date;
		}

		//Controllo se il titolo è vuoto
		if(TitoloEntry.Text == null || TitoloEntry.Text.Trim() == "")
		{
			DisplayAlert("Errore", "Inserire un titolo per l'attività", "OK");
			return;
		}

		//controllo datafine in modo che non sia minore di quella d'inizio
		if (dataFine.HasValue && dataFine.Value < dataInizio)
		{
			DisplayAlert("Errore", "La data di fine non può essere prima di quella d'inizio", "OK");
			return;
		}

		//Creo l'attività
		var nuova = new Attivita
		{
			Titolo = TitoloEntry.Text,
			Data = DataSelezionata,
			DataFine = dataFine,
			Colore = colore
		};

		//La aggiungo alla lista generale
		tutteLeAttivita.Add(nuova);

		DateTime fine = dataFine ?? dataInizio; //Se non c'è data fine prende la data di inizio

		//Aggiungo l'attività ad EventiCalendario per ogni giorno in cui è presente
		for(DateTime d = dataInizio; d <= fine; d = d.AddDays(1))
		{
            if (EventiCalendario.ContainsKey(d))
            {
				//Add diretto impossibile perchè restituisce IEnumerable normalmente
                var lista = EventiCalendario[d].Cast<object>().ToList();  //Prende la lista e la converte in una List<object>
                lista.Add(nuova);  //Aggiungo l'attività
                EventiCalendario[d] = lista;  //Metto la lista nell'indice della collezione
            }
            else
            {
				//Se il giorno non esiste ancora nella collection
                EventiCalendario.Add(d, new List<object> { nuova });  //Creo il giorno nella collection e metto dentro una lista con l'attività
            }
        }

		//Aggiorno la lista del giorno
		AggiornaAttivita(DataSelezionata);

		Overlay.IsVisible = false;

		TitoloEntry.Text = "";
	}

	void OnDataFineSwitchToggled(object sender, ToggledEventArgs e)
	{
		DataFinePicker.IsEnabled = e.Value;
		DataFinePicker.Opacity = e.Value ? 1 : 0.5;
	}

	//Async void perchè uso un await che aspetta la conferma dell'utente
	async void EliminaAttivita(object sender, EventArgs e)
	{
		var button = sender as Button;
		var attivita = button?.CommandParameter as Attivita; //Se button non è null prende il CommandParameter

		//Leggo la scelta dell'utente
		bool conferma = await DisplayAlert("Conferma", "Vuoi eliminare questa attività?", "Sì", "No");

		if(!conferma)
			return;

		if (attivita == null)
			return;

		//Rimozione dalla lista principale
		tutteLeAttivita.Remove(attivita);

		//Rimuove dal calendario per tutti i giorni
		DateTime fine = attivita.DataFine ?? attivita.Data;

		for(DateTime d = attivita.Data; d <= fine; d = d.AddDays(1))
		{
			if(EventiCalendario.ContainsKey(d))
			{
				var lista = EventiCalendario[d].Cast<object>().ToList();  //Prendo la lista del giorno

				lista.Remove(attivita);  //Rimuovo l'attività

				if(lista.Count == 0)
					EventiCalendario.Remove(d); //Se la lista è vuota rimuovo il giorno dalla collection
				else
					EventiCalendario[d] = lista;
			}
		}

		AggiornaAttivita(DataSelezionata);
	}

	void OnSearchPressed(object sender, EventArgs e)
	{
		string testo = SearchBarAttivita.Text?.ToLower().Trim() ?? "";  //Prendo il testo di ricerca in piccolo

		RisultatiRicerca.Clear();  //Svuoto i risultati di prima

		// Filtro tutte le attività controllando se contengono il testo di ricerca
		var risultati = tutteLeAttivita
			.Where(a => a.Titolo.ToLower().Contains(testo))
			.ToList();

		//Aggiungo ogni risultato alla lista di risultati di ricerca
		foreach (var a in risultati)
		{
			RisultatiRicerca.Add(a);
		}

		//Controllo se ci sono risultati o no
		NessunRisultato = risultati.Count == 0;

		//Rendo l'overlay visibile
		SearchOverlay.IsVisible = true;
	}

	void ChiudiSearchOverlay(object sender, EventArgs e)
	{
		SearchOverlay.IsVisible = false;
	}

	void OnRisultatoSelezionato(object sender, SelectionChangedEventArgs e)
	{
		//Prende l'attività scelta
		var attivita = e.CurrentSelection.FirstOrDefault() as Attivita;

		if(attivita == null) 
			return;

		//Cambia il giorno selezionato
		DataSelezionata = attivita.Data;

		//Chiude overlay
		SearchOverlay.IsVisible = false;

		//Resetta la selezione
		(sender as CollectionView).SelectedItem = null;
	}

	void OnAttivitaSelezionata(object sender, SelectionChangedEventArgs e)
	{
		var attivita = e.CurrentSelection.FirstOrDefault() as Attivita;

		if(attivita == null) 
			return;

		DettaglioHeader.BackgroundColor = attivita.Colore;

		DettaglioTitolo.Text = attivita.Titolo;

		DettaglioDataInizio.Text = attivita.Data.ToString("dd MMMM yyyy");

		//Se non c'è una data fine, mostra "nessuna"
		DettaglioDataFine.Text = attivita.DataFine.HasValue
			? attivita.DataFine.Value.ToString("dd MMMM yyyy")
			: "Nessuna";

		//Pallino colorato con nome del colore
		DettaglioColore.Color = attivita.Colore;
		DettaglioColoreNome.Text = attivita.Colore == Colors.Red ? "Rosso"
            : attivita.Colore == Colors.Green ? "Verde"
            : attivita.Colore == Colors.Blue ? "Blu"
            : attivita.Colore.ToString();

		OverlayDettaglio.IsVisible = true;

		//Resetta la selezione
		(sender as CollectionView).SelectedItem = null;
    }

	void ChiudiOverlayDettaglio(object sender, EventArgs e)
	{
		OverlayDettaglio.IsVisible = false;
	}
}