using OraX.Models;
using OraX.Services;

namespace OraX;

public partial class RegisterPage : ContentPage
{

    DatabaseService database;

    public RegisterPage(DatabaseService db)
    {
        InitializeComponent();
        database = db;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {

        var userEsistente = await database.GetUserByUsername(UsernameEntry.Text);

        if (userEsistente != null)
        {
            await DisplayAlert("Errore", "Username già esistente", "OK");
            return;
        }

        User nuovoUser = new User
        {
            Nome = NomeEntry.Text,
            Cognome = CognomeEntry.Text,
            DataNascita = DataNascitaPicker.Date,
            Username = UsernameEntry.Text,
            PasswordHash = PasswordHelper.HashPassword(PasswordEntry.Text),
            DataRegistrazione = DateTime.Now
        };

        await database.RegistraUser(nuovoUser);

        await DisplayAlert("Successo", "Account creato", "OK");

        await Navigation.PopAsync();
    }
}