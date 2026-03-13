using System.Collections.ObjectModel;
using System.ComponentModel;
using Plugin.Maui.Calendar.Models;

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
			if (a.Data.Date == giorno.Date)
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

		var nuova = new Attivita
		{
			Titolo = TitoloEntry.Text,
			Data = DataSelezionata,
			Colore = colore
		};

		tutteLeAttivita.Add(nuova);

		if(EventiCalendario.ContainsKey(DataSelezionata))
		{
            var lista = EventiCalendario[DataSelezionata].Cast<object>().ToList();
            lista.Add(nuova);
            EventiCalendario[DataSelezionata] = lista;
        }
        else
		{
			EventiCalendario.Add(DataSelezionata, new List<object> { nuova });
		}

			AggiornaAttivita(DataSelezionata);

		Overlay.IsVisible = false;

		TitoloEntry.Text = "";
	}
}