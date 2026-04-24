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

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Errore", "Inserisci username e password", "OK");
            return;
        }

        string hash = PasswordHelper.HashPassword(password);

        User user = await database.GetUser(username, hash);

        if (user == null)
        {
            await DisplayAlert("Errore", "Credenziali errate", "OK");
            return;
        }
;
        UserSession.CurrentUser = user;

        await DisplayAlert("OK", $"Benvenuto {user.Nome}", "OK");

    
        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void OnRegisterTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(database));
    }
}