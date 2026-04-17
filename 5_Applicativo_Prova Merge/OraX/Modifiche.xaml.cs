using OraX.Services;
using OraX.Models;

namespace OraX;

public partial class Modifiche : ContentPage
{
    User user;
    DatabaseService database;

    public Modifiche()
    {
        InitializeComponent();

        user = UserSession.CurrentUser;

        if (user != null)
        {
            NameEntry.Text = user.Nome;
            SurnameEntry.Text = user.Cognome;
            UsernameEntry.Text = user.Username;
            EmailEntry.Text = user.Email;
            PhoneEntry.Text = user.Telefono;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (user == null)
        {
            await DisplayAlert("Errore", "Utente non trovato", "OK");
            return;
        }

        user.Nome = NameEntry.Text;
        user.Cognome = SurnameEntry.Text;
        user.Email = EmailEntry.Text;
        user.Telefono = PhoneEntry.Text;

        await DisplayAlert("OK", "Modifiche salvate (DB da implementare update)", "OK");
    }
}