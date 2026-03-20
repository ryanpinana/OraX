using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Plugin.Maui.Calendar.Models;
using System.Globalization;

namespace OraX;

public partial class CalendarPageV2 : ContentPage, INotifyPropertyChanged
{
	DateTime dataSelezionata;

	public DateTime DataSelezionata
	{
		get => dataSelezionata;

		set
		{
			if(dataSelezionata != value)
			{
				dataSelezionata = value;
				OnPropertyChanged();

				AggiornaAttivita(value);
			}
		}
	}
	public ObservableCollection<Attivita> AttivitaDelGiorno { get; set; } = new();

	public EventCollection EventiCalendario { get; set; } = new();

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

	void AggiornaAttivita(DateTime giorno)
	{
		AttivitaDelGiorno.Clear();

		foreach (var a in tutteLeAttivita)
		{
			DateTime fine = a.DataFine ?? a.Data;

			if (giorno.Date >= a.Data.Date && giorno.Date <= fine.Date)
			{
				AttivitaDelGiorno.Add(a);
			}
		}

	}

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
		Color colore = Colors.Red;

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

		if(DataFineSwitch.IsToggled)
		{
			dataFine = DataFinePicker.Date;
		}

		//controllo data
		if (dataFine.HasValue && dataFine.Value < dataInizio)
		{
			DisplayAlert("Errore", "La data di fine non può essere prima di quella d'inizio", "OK");
			return;
		}

		var nuova = new Attivita
		{
			Titolo = TitoloEntry.Text,
			Data = DataSelezionata,
			DataFine = dataFine,
			Colore = colore
		};

		tutteLeAttivita.Add(nuova);

		DateTime fine = dataFine ?? dataInizio;

		for(DateTime d = dataInizio; d <= fine; d = d.AddDays(1))
		{
            if (EventiCalendario.ContainsKey(d))
            {
                var lista = EventiCalendario[d].Cast<object>().ToList();
                lista.Add(nuova);
                EventiCalendario[d] = lista;
            }
            else
            {
                EventiCalendario.Add(d, new List<object> { nuova });
            }
        }

		AggiornaAttivita(DataSelezionata);

		Overlay.IsVisible = false;

		TitoloEntry.Text = "";
	}

	void OnDataFineSwitchToggled(object sender, ToggledEventArgs e)
	{
		DataFinePicker.IsEnabled = e.Value;
		DataFinePicker.Opacity = e.Value ? 1 : 0.5;
	}

	async void EliminaAttivita(object sender, EventArgs e)
	{
		var button = sender as Button;
		var attivita = button?.CommandParameter as Attivita;

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
				var lista = EventiCalendario[d].Cast<object>().ToList();

				lista.Remove(attivita);

				if(lista.Count == 0)
					EventiCalendario.Remove(d);
				else
					EventiCalendario[d] = lista;
			}
		}

		AggiornaAttivita(DataSelezionata);
	}
}