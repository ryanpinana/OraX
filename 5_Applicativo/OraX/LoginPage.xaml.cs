using OraX.Services;
using OraX.Models;

namespace OraX;

public partial class LoginPage : ContentPage
{
    DatabaseService database;

    public LoginPage(DatabaseService db)
    {
        InitializeComponent();
        database = db;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("DB PATH: " + FileSystem.AppDataDirectory);
    }

    private async void OnRegisterTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(database));
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Errore", "Inserisci username e password", "OK");
            return;
        }

        string hashPassword = PasswordHelper.HashPassword(password);

        User user = await database.GetUser(username, hashPassword);

        if (user == null)
        {
            await DisplayAlert("Errore", "Username o password sbagliati", "OK");
            return;
        }

        await DisplayAlert("Login riuscito", "Benvenuto/a " + user.Nome + user.Cognome, "OK");
    }
}